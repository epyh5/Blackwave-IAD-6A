using UnityEngine;
using System.Collections;

public class MusicaGeneral : MonoBehaviour
{
    [Header("Configuración")]
    public AudioClip cancion;
    [Range(0f, 1f)]
    public float volumen = 0.7f;
    public bool reproducirEnLoop = true;
    public bool reproducirAlInicio = true;
    
    [Header("Efectos")]
    public bool usarFadeIn = true;
    public float tiempoFadeIn = 2f;
    public bool pararOtraMusica = true;
    
    private AudioSource audioSource;
    
    private void Start()
    {
        ConfigurarAudio();
        
        if (reproducirAlInicio)
        {
            ReproducirMusica();
        }
    }
    
    private void ConfigurarAudio()
    {
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        
        audioSource.clip = cancion;
        audioSource.volume = usarFadeIn ? 0f : volumen;
        audioSource.loop = reproducirEnLoop;
        audioSource.playOnAwake = false;
    }
    
    public void ReproducirMusica()
    {
        if (cancion == null) return;
        
        
        if (pararOtraMusica)
        {
            AudioSource[] otrosAudios = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audio in otrosAudios)
            {
                if (audio != audioSource && audio.isPlaying)
                {
                    audio.Stop();
                }
            }
        }
        
        // Reproducir
        audioSource.Play();
        
        if (usarFadeIn)
        {
            StartCoroutine(FadeIn());
        }
    }
    
    public void PararMusica()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    
    private IEnumerator FadeIn()
    {
        float tiempo = 0f;
        
        while (tiempo < tiempoFadeIn)
        {
            audioSource.volume = Mathf.Lerp(0f, volumen, tiempo / tiempoFadeIn);
            tiempo += Time.deltaTime;
            yield return null;
        }
        
        audioSource.volume = volumen;
    }
}