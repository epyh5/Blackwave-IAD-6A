using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Configuración del Portal")]
    [SerializeField] private string escenaDestino = "Final";
    
    [Header("Efectos de Sonido")]
    [SerializeField] private AudioClip sonidoActivacion;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Configuración")]
    [SerializeField] private float tiempoEspera = 1f; // Tiempo antes de cambiar escena
    
    private bool cambiandoEscena = false;
    
    private void Start()
    {
        // Configurar AudioSource si no está asignado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !cambiandoEscena)
        {
            Debug.Log("¡Jugador entró al portal!");
            CambiarEscena();
        }
    }
    
    // Este método ya no es necesario, pero lo dejo comentado por si acaso
    /*
    private void OnTriggerExit2D(Collider2D other)
    {
        // No necesitamos este método para activación automática
    }
    */
    
    // Ya no necesitamos Update para input
    /*
    private void Update()
    {
        // No hay input requerido
    }
    */
    
    private void CambiarEscena()
    {
        if (cambiandoEscena) return;
        
        cambiandoEscena = true;
        Debug.Log("Cambiando a escena: " + escenaDestino);
        
        // Reproducir sonido de activación con DontDestroyOnLoad
        if (sonidoActivacion != null)
        {
            // Crear un AudioSource temporal que persista entre escenas
            GameObject tempAudio = new GameObject("PortalSound");
            DontDestroyOnLoad(tempAudio);
            
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = sonidoActivacion;
            tempSource.volume = 0.8f;
            tempSource.Play();
            
            // Destruir después de que termine el sonido
            Destroy(tempAudio, sonidoActivacion.length + 0.1f);
            
            // Esperar un poco más para que el sonido se reproduzca
            float tiempoEsperaConSonido = Mathf.Max(tiempoEspera, 0.3f);
            Invoke("LoadScene", tiempoEsperaConSonido);
        }
        else
        {
            // Si no hay sonido, cambiar inmediatamente
            Invoke("LoadScene", 0.1f);
        }
    }
    
    private void LoadScene()
    {
        SceneManager.LoadScene(escenaDestino);
    }
    
    // Método para activar el portal desde otros scripts
    public void ActivarPortal()
    {
        CambiarEscena();
    }
}