using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Configuración")]
    public bool esCheckpointInicial = false;
    public GameObject efectoActivacion; // Opcional: efecto visual
    
    private bool yaActivado = false;
    
    private void Start()
    {
        if (esCheckpointInicial)
        {
            // Buscar al jugador y establecer checkpoint inicial
            VidaJugador jugador = FindObjectOfType<VidaJugador>();
            if (jugador != null)
            {
                jugador.EstablecerCheckpoint(this.transform);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !yaActivado)
        {
            ActivarCheckpoint(other.GetComponent<VidaJugador>());
        }
    }
    
    void ActivarCheckpoint(VidaJugador jugador)
    {
        if (jugador == null) return;
        
        yaActivado = true;
        jugador.EstablecerCheckpoint(this.transform);
        
        // Efectos visuales opcionales
        if (efectoActivacion != null)
        {
            efectoActivacion.SetActive(true);
        }
        
        Debug.Log("¡Checkpoint activado!");
    }
}
