using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class SonidosJugador : MonoBehaviour
{
    [Header("Sonidos")]
    public AudioClip sonidoSalto;
    public AudioClip sonidoCorrer;
    public AudioClip sonidoDaño;
    public AudioClip sonidoCuración;
    
    [Header("Configuración")]
    [Range(0f, 1f)]
    public float volumen = 0.7f;
    [Range(0f, 1f)]
    public float volumenDaño = 0.8f;
    [Range(0f, 1f)]
    public float volumenCuración = 0.6f;

    private AudioSource audioSource;
    private AudioSource audioSourceEfectos;
    private bool estaCorriendo = false;
    private PlayerInput playerInput;
    private InputAction accionMovimiento;
    private InputAction accionSalto;

    private void Start()
    {
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volumen;

        
        audioSourceEfectos = gameObject.AddComponent<AudioSource>();
        audioSourceEfectos.volume = volumen;

        
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
        }

        
        accionMovimiento = playerInput.actions["Move"];
        accionSalto = playerInput.actions["Jump"];

        
        if (accionSalto != null)
        {
            accionSalto.performed += OnSalto;
        }
    }

    private void Update()
    {
        if (accionMovimiento != null)
        {
            
            Vector2 movimiento = accionMovimiento.ReadValue<Vector2>();
            bool seEstaMoviendo = Mathf.Abs(movimiento.x) > 0.1f;

            // Sonido de correr
            if (seEstaMoviendo && !estaCorriendo)
            {
                ReproducirSonidoCorrer();
                estaCorriendo = true;
            }
            else if (!seEstaMoviendo && estaCorriendo)
            {
                DetenerSonidoCorrer();
                estaCorriendo = false;
            }
        }
    }

    private void OnSalto(InputAction.CallbackContext context)
    {
        ReproducirSonidoSalto();
    }

    public void ReproducirSonidoSalto()
    {
        if (sonidoSalto != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoSalto);
        }
    }

    public void ReproducirSonidoCorrer()
    {
        if (sonidoCorrer != null && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.clip = sonidoCorrer;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void DetenerSonidoCorrer()
    {
        if (audioSource != null && audioSource.isPlaying && audioSource.clip == sonidoCorrer)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    
    public void ReproducirSonidoDaño()
    {
        if (sonidoDaño != null && audioSourceEfectos != null)
        {
            audioSourceEfectos.Stop();
            audioSourceEfectos.volume = volumenDaño;
            audioSourceEfectos.PlayOneShot(sonidoDaño);
            
            Debug.Log("¡Jugador recibió daño! - Sonido reproducido");
        }
    }
    
    
    public void ReproducirSonidoCuración()
    {
        if (sonidoCuración != null && audioSourceEfectos != null)
        {
            audioSourceEfectos.Stop();
            audioSourceEfectos.volume = volumenCuración;
            audioSourceEfectos.PlayOneShot(sonidoCuración);
            
            Debug.Log("¡Jugador se curó! - Sonido reproducido");
        }
    }
    
    
    public void ReproducirSonidoDañoConDelay(float delay)
    {
        if (sonidoDaño != null)
        {
            StartCoroutine(ReproducirSonidoConDelay(sonidoDaño, delay, volumenDaño));
        }
    }
    
    
    public void ReproducirSonidoCuraciónConDelay(float delay)
    {
        if (sonidoCuración != null)
        {
            StartCoroutine(ReproducirSonidoConDelay(sonidoCuración, delay, volumenCuración));
        }
    }
    
    private IEnumerator ReproducirSonidoConDelay(AudioClip clip, float delay, float volumenEspecifico)
    {
        yield return new WaitForSeconds(delay);
        
        if (clip != null && audioSourceEfectos != null)
        {
            audioSourceEfectos.Stop();
            audioSourceEfectos.volume = volumenEspecifico;
            audioSourceEfectos.PlayOneShot(clip);
        }
    }
    
    
    public void CambiarVolumenGeneral(float nuevoVolumen)
    {
        volumen = Mathf.Clamp01(nuevoVolumen);
        if (audioSource != null)
            audioSource.volume = volumen;
        if (audioSourceEfectos != null)
            audioSourceEfectos.volume = volumen;
    }
    
    private void OnDestroy()
    {
        
        if (accionSalto != null)
        {
            accionSalto.performed -= OnSalto;
        }
    }
}