using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;

public class VidaJugador : MonoBehaviour
{
    [Header("Sistema de Vidas")]
    public int vidasMaximas = 3;
    private int vidasActuales;
    
    [Header("Checkpoints")]
    public Transform checkpointActual;
    public Transform posicionInicial;
    
    [Header("Invencibilidad")]
    public float tiempoInvencibilidad = 1.5f;
    public float frecuenciaParpadeo = 0.1f;
    
    [Header("Eventos")]
    public UnityEvent<int> OnVidaCambiada;
    public UnityEvent OnMuerte;
    
    [Header("Debug")]
    public bool mostrarDebug = true;
    
    private bool esInvencible = false;
    private SpriteRenderer spriteRenderer;
    private MovimientoJugador movimientoJugador;
    private Animator animator;
    
    private void Awake()
    {
        vidasActuales = vidasMaximas;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        movimientoJugador = GetComponent<MovimientoJugador>();
        animator = GetComponent<Animator>();
        
        if (checkpointActual == null && posicionInicial != null)
            checkpointActual = posicionInicial;
        else if (checkpointActual == null)
            checkpointActual = transform;
        
        if (mostrarDebug)
            Debug.Log($"=== VidaJugador iniciado - ESCENA: {SceneManager.GetActiveScene().name} - Vidas: {vidasActuales}/{vidasMaximas} ===");
    }
    
    public void RecibirDaño(int cantidad = 1)
    {
        
        Debug.Log($"=== RECIBIR DAÑO - ESCENA: {SceneManager.GetActiveScene().name} ===");
        Debug.Log($"Vidas ANTES del daño: {vidasActuales}/{vidasMaximas}");
        
        if (esInvencible) 
        {
            if (mostrarDebug)
                Debug.Log("Jugador es invencible, daño ignorado");
            return;
        }
        
        vidasActuales -= cantidad;
        
        Debug.Log($"Vidas DESPUÉS del daño: {vidasActuales}/{vidasMaximas}");
        
        // Activar animación de daño
        if (movimientoJugador != null)
            movimientoJugador.ActivarAnimacionDaño();
        
        OnVidaCambiada?.Invoke(vidasActuales);
        
        string escenaActual = SceneManager.GetActiveScene().name;
        
        
        if (escenaActual == "Nivel1")
        {
            if (vidasActuales <= 0)
            {
                
                Debug.Log($"=== NIVEL1: SIN VIDAS - REINICIANDO COMPLETAMENTE ===");
                ReiniciarNivel1();
            }
            else
            {
                
                Debug.Log($"=== NIVEL1: RESPAWN CON {vidasActuales} VIDAS ===");
                RespawnEnCheckpoint();
            }
        }
        else if (escenaActual == "Nivel2")
        {
            // EN NIVEL2: SISTEMA DE VIDAS CON CHECKPOINTS
            if (vidasActuales <= 0)
            {
                
                if (mostrarDebug)
                    Debug.Log("En Nivel2 - Sin vidas, regresando a Nivel1");
                RegresarAlNivel1();
            }
            else
            {
                
                if (mostrarDebug)
                    Debug.Log($"En Nivel2 - Respawn en checkpoint. Vidas: {vidasActuales}");
                RespawnEnCheckpoint();
            }
        }
        else
        {
            
            RegresarAlNivel1();
        }
    }
    
    private void MuerteDirectaAlMenu()
    {
        OnMuerte?.Invoke();
        
        if (animator != null)
            animator.SetTrigger("morir");
        
        if (mostrarDebug)
            Debug.Log("Cargando MenuScene...");
        
        Invoke(nameof(CargarMenu), 1.5f);
    }
    
    private void RegresarAlNivel1()
    {
        OnMuerte?.Invoke();
        
        if (animator != null)
            animator.SetTrigger("morir");
        
        if (mostrarDebug)
            Debug.Log("Regresando a Nivel1...");
        
        
        vidasActuales = vidasMaximas;
        
        Invoke(nameof(CargarNivel1), 1.5f);
    }
    
    private void RespawnEnCheckpoint()
    {
        if (checkpointActual != null)
        {
            transform.position = checkpointActual.position;
            
            
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
        
        StartCoroutine(PeriodoInvencibilidad());
    }
    
    private void ReiniciarNivel1()
    {
        if (animator != null)
            animator.SetTrigger("morir");
        
        if (mostrarDebug)
            Debug.Log("Reiniciando Nivel1...");
        
        // SÍ restaurar vidas al máximo cuando reinicia Nivel1
        vidasActuales = vidasMaximas;
        
        Invoke(nameof(CargarNivel1), 1.5f);
    }
    
    private void CargarMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
    
    private void CargarNivel1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Nivel1");
    }
    
    // MÉTODO PARA EL TEMPORIZADOR
    public void TiempoAgotado()
    {
        if (mostrarDebug)
            Debug.Log("¡Tiempo agotado! Forzando regreso a Nivel1...");
        
        RegresarAlNivel1();
    }
    
    public void EstablecerCheckpoint(Transform nuevoCheckpoint)
    {
        checkpointActual = nuevoCheckpoint;
        if (mostrarDebug)
            Debug.Log($"Nuevo checkpoint establecido: {nuevoCheckpoint.name}");
    }
    
    public void Curar(int cantidad)
    {
        vidasActuales = Mathf.Min(vidasActuales + cantidad, vidasMaximas);
        OnVidaCambiada?.Invoke(vidasActuales);
        
        if (mostrarDebug)
            Debug.Log($"Jugador curado +{cantidad}. Vidas: {vidasActuales}/{vidasMaximas}");
    }
    
    private IEnumerator PeriodoInvencibilidad()
    {
        esInvencible = true;
        float tiempoTranscurrido = 0;
        
        while (tiempoTranscurrido < tiempoInvencibilidad)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(frecuenciaParpadeo);
            
            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(frecuenciaParpadeo);
            
            tiempoTranscurrido += frecuenciaParpadeo * 2;
        }
        
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
        esInvencible = false;
        
        if (mostrarDebug)
            Debug.Log("Invencibilidad terminada");
    }
    
   
    public int GetVidasActuales() => vidasActuales;
    public int GetVidasMaximas() => vidasMaximas;
    public int GetVidaActual() => vidasActuales;
    public int GetVidaMaxima() => vidasMaximas;
    public bool EstaInvencible() => esInvencible;
    public bool EstaMuerto() => vidasActuales <= 0;
}