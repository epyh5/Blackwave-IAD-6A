using UnityEngine;

public class Trampa : MonoBehaviour
{
    [Header("Configuración")]
    public int dañoCausado = 1;
    public bool mostrarDebug = true;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            VidaJugador vidaScript = other.GetComponent<VidaJugador>();
            if (vidaScript != null)
            {
                if (mostrarDebug)
                    Debug.Log($"¡{other.name} tocó la trampa! Causando {dañoCausado} de daño");
                
                vidaScript.RecibirDaño(dañoCausado);
            }
            else if (mostrarDebug)
            {
                Debug.LogWarning("El jugador no tiene el componente VidaJugador");
            }
        }
    }
}