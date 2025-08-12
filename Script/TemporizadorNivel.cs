using UnityEngine;
using UnityEngine.UI;

public class TemporizadorNivel : MonoBehaviour
{
    [Header("Configuración del Timer")]
    public float tiempoLimite = 60f;
    public Text textoTiempo;
    public TMPro.TextMeshProUGUI textoTiempoTMP;
    
    [Header("Debug")]
    public bool mostrarDebug = true;
    
    private float tiempoRestante;
    private bool tiempoAgotado = false;
    private VidaJugador vidaJugador;
    
    private void Start()
    {
        tiempoRestante = tiempoLimite;
        
        // Buscar el jugador
        vidaJugador = FindObjectOfType<VidaJugador>();
        if (vidaJugador == null)
        {
            Debug.LogError("TemporizadorNivel: No se encontró VidaJugador!");
        }
        
        if (mostrarDebug)
            Debug.Log($"Temporizador iniciado: {tiempoLimite} segundos");
    }
    
    private void Update()
    {
        if (!tiempoAgotado && !JuegoTerminado())
        {
            tiempoRestante -= Time.deltaTime;
            
            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                TiempoAgotado();
            }
            
            ActualizarUI();
        }
    }
    
    void ActualizarUI()
    {
        int minutos = Mathf.FloorToInt(tiempoRestante / 60);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60);
        string tiempoTexto = string.Format("{0:00}:{1:00}", minutos, segundos);
        
        // Cambiar color según tiempo restante
        Color color = Color.white;
        if (tiempoRestante <= 10f)
            color = Color.red;
        else if (tiempoRestante <= 20f)
            color = Color.yellow;
        
        // Efecto de parpadeo en los últimos 10 segundos
        if (tiempoRestante <= 10f && tiempoRestante > 0)
        {
            float alpha = Mathf.PingPong(Time.time * 2f, 1f);
            color.a = Mathf.Lerp(0.6f, 1f, alpha);
        }
        
        if (textoTiempo != null)
        {
            textoTiempo.text = "Tiempo: " + tiempoTexto;
            textoTiempo.color = color;
        }
            
        if (textoTiempoTMP != null)
        {
            textoTiempoTMP.text = "Tiempo: " + tiempoTexto;
            textoTiempoTMP.color = color;
        }
    }
    
    void TiempoAgotado()
    {
        tiempoAgotado = true;
        
        if (mostrarDebug)
            Debug.Log("¡Tiempo agotado! Activando muerte por tiempo...");
        
        if (vidaJugador != null)
        {
            
            vidaJugador.TiempoAgotado();
        }
        else
        {
            
            Debug.LogWarning("VidaJugador no encontrado - fallback directo");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Nivel1");
        }
    }
    
   
    bool JuegoTerminado()
    {
        // Si el jugador ya murió
        if (vidaJugador != null && vidaJugador.EstaMuerto())
            return true;
            
        // Si se completó el nivel
        if (GameManager.Instance != null && GameManager.Instance.TodosNPCsCurados())
            return true;
        
        return false;
    }
    
    
    public void PausarTiempo()
    {
        enabled = false;
        if (mostrarDebug) Debug.Log("Timer pausado");
    }
    
    public void ReanudarTiempo()
    {
        enabled = true;
        if (mostrarDebug) Debug.Log("Timer reanudado");
    }
    
    public void AgregarTiempo(float segundos)
    {
        tiempoRestante += segundos;
        if (mostrarDebug) Debug.Log($"Tiempo agregado: +{segundos}s");
    }
    
    public float GetTiempoRestante() => tiempoRestante;
    public bool EsTiempoAgotado() => tiempoAgotado;
    
    
    [ContextMenu("Debug: 10 segundos")]
    void DebugForzar10Segundos()
    {
        tiempoRestante = 10f;
        Debug.Log("Tiempo forzado a 10 segundos");
    }
    
    [ContextMenu("Debug: Agotar tiempo")]
    void DebugAgotarTiempo()
    {
        TiempoAgotado();
    }
}