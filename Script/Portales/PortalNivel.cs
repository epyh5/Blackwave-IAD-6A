using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalNivel : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreEscenaDestino = "Nivel2";
    
    [Header("Debug")]
    public bool mostrarDebug = true;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (mostrarDebug)
                Debug.Log($"Cambiando a {nombreEscenaDestino}");
            
            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}
