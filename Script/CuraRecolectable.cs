using UnityEngine;

public class CuraRecolectable : MonoBehaviour
{
    [Header("Debug")]
    public bool mostrarDebug = true;
    
    [Header("Efectos")]
    public AudioClip sonidoRecoleccion;
    public GameObject efectoParticulas;
    
    private bool yaRecolectada = false;
    
    void Start()
    {
        if (mostrarDebug)
            Debug.Log($"Cura {gameObject.name} lista para recolectar");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (mostrarDebug)
            Debug.Log($"Algo tocó la cura: {other.gameObject.name}");
        
        if (other.CompareTag("Player") && !yaRecolectada)
        {
            RecolectarCura();
        }
    }
    
    void RecolectarCura()
    {
        if (yaRecolectada) return;
        
        yaRecolectada = true;
        
        if (mostrarDebug)
            Debug.Log("¡Recolectando cura!");
        
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("¡GameManager no encontrado! ¿Está en la escena?");
            return;
        }
        
        // Avisar al GameManager
        GameManager.Instance.RecolectarCura();
        
        
        ReproducirEfectos();
        
        // Destruir el objeto
        Destroy(gameObject);
    }
    
    void ReproducirEfectos()
    {
        // Reproducir sonido
        if (sonidoRecoleccion != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecoleccion, transform.position);
        }
        
        
        if (efectoParticulas != null)
        {
            Instantiate(efectoParticulas, transform.position, transform.rotation);
        }
    }
}
