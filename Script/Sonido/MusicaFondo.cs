using UnityEngine;


public class MusicaFondo : MonoBehaviour
{

    public AudioClip musicaNivel;

    public float volumen = 0.5f;
    
    private AudioSource audioSource;

    private void Start()
    {
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicaNivel;
        audioSource.loop = true;  
        audioSource.volume = volumen;
        
        
        if (musicaNivel != null)
        {
            audioSource.Play();
        }
    }
}