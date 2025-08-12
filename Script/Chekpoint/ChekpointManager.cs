
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class NivelManager : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    public string nombreNivel = "Level2";
    public bool esNivel2 = false; // Activar solo en el nivel 2
    
    [Header("Timer (Solo Nivel 2)")]
    public float tiempoLimite = 60f; // 60 segundos por ejemplo
    public bool timerActivo = true;
    private float tiempoRestante;
    
    [Header("Checkpoint")]
    private Vector3 posicionCheckpoint;
    private bool tieneCheckpointActivo = false;
    
    [Header("Referencias")]
    public Transform jugador;
    
    [Header("UI")]
    public UnityEngine.UI.Text textoTimer; // Opcional
    
    [Header("Debug")]
    public bool mostrarDebug = true;
    
    void Start()
    {
        // Encontrar al jugador si no está asignado
        if (jugador == null)
        {
            GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
            if (jugadorObj != null)
                jugador = jugadorObj.transform;
        }
        
        // Configurar timer solo para nivel 2
        if (esNivel2 && timerActivo)
        {
            tiempoRestante = tiempoLimite;
            StartCoroutine(ContadorTiempo());
        }
        
        // Establecer checkpoint inicial (posición de inicio)
        if (jugador != null)
        {
            posicionCheckpoint = jugador.position;
            tieneCheckpointActivo = true;
            
            if (mostrarDebug)
                Debug.Log($"Checkpoint inicial establecido en: {posicionCheckpoint}");
        }
        
        // Suscribirse al evento de muerte del jugador
        VidaJugador vidaJugador = jugador != null ? jugador.GetComponent<VidaJugador>() : null;
        if (vidaJugador != null)
        {
            vidaJugador.OnMuerte.AddListener(OnJugadorMuerto);
        }
    }
    
    void Update()
    {
        // Actualizar UI del timer
        if (esNivel2 && textoTimer != null && timerActivo)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoTimer.text = $"Tiempo: {minutos:00}:{segundos:00}";
        }
    }
    
    IEnumerator ContadorTiempo()
    {
        while (tiempoRestante > 0 && timerActivo)
        {
            yield return new WaitForSeconds(1f);
            tiempoRestante -= 1f;
            
            // Advertencias
            if (tiempoRestante <= 10f && tiempoRestante > 9f)
            {
                if (mostrarDebug)
                    Debug.Log("10 segundos restantes!");
            }
        }
        
        // Tiempo agotado
        if (tiempoRestante <= 0)
        {
            TiempoAgotado();
        }
    }
    
    void TiempoAgotado()
    {
        if (mostrarDebug)
            Debug.Log("Tiempo agotado! Regresando al Level1...");
        
        timerActivo = false;
        RegresarAlPrimerNivel();
    }
    
    public void EstablecerCheckpoint(Vector3 nuevaPosicion)
    {
        posicionCheckpoint = nuevaPosicion;
        tieneCheckpointActivo = true;
        
        if (mostrarDebug)
            Debug.Log($"Checkpoint actualizado: {posicionCheckpoint}");
    }
    
    public void OnJugadorMuerto()
    {
        if (mostrarDebug)
            Debug.Log("Jugador murio...");
        
        if (esNivel2 && tieneCheckpointActivo)
        {
            // En nivel 2: usar checkpoint si existe, sino ir al Level1
            StartCoroutine(RespawnEnCheckpoint());
        }
        else
        {
            // En cualquier otro nivel: ir al Level1
            RegresarAlPrimerNivel();
        }
    }
    
    IEnumerator RespawnEnCheckpoint()
    {
        yield return new WaitForSeconds(1f);
        
        if (jugador != null && tieneCheckpointActivo)
        {
            // Mover al jugador al checkpoint
            jugador.position = posicionCheckpoint;
            
            // Restaurar vida del jugador
            VidaJugador vidaJugador = jugador.GetComponent<VidaJugador>();
            if (vidaJugador != null)
            {
                // Resetear vida a máximo
                vidaJugador.Curar(vidaJugador.GetVidaMaxima());
            }
            
            if (mostrarDebug)
                Debug.Log($"Jugador respawneado en checkpoint: {posicionCheckpoint}");
        }
        else
        {
            RegresarAlPrimerNivel();
        }
    }
    
    void RegresarAlPrimerNivel()
    {
        if (mostrarDebug)
            Debug.Log("Regresando al Level1...");
        
        // Detener timer
        timerActivo = false;
        
        // Cargar primer nivel
        SceneManager.LoadScene("Level1"); // Cambia por el nombre exacto
    }
    
    // Métodos públicos para control externo
    public void PausarTimer() => timerActivo = false;
    public void ReanudarTimer() => timerActivo = true;
    public float GetTiempoRestante() => tiempoRestante;
    public void AgregarTiempo(float segundos) => tiempoRestante += segundos;
    
    void OnDrawGizmos()
    {
        if (tieneCheckpointActivo)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(posicionCheckpoint, 0.5f);
            Gizmos.DrawLine(posicionCheckpoint, posicionCheckpoint + Vector3.up * 2f);
        }
        
        // Mostrar información del timer
        if (esNivel2 && Application.isPlaying)
        {
            Gizmos.color = tiempoRestante > 10 ? Color.green : Color.red;
            Vector3 pos = transform.position + Vector3.up * 3f;
            
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(pos, $"Tiempo: {tiempoRestante:F1}s");
            #endif
        }
    }
}