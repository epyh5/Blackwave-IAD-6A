using UnityEngine;
using UnityEngine.UI;

public class UIVidas : MonoBehaviour
{
    [Header("Referencias UI")]
    public Text textoVidas; 
    public TMPro.TextMeshProUGUI textoVidasTMP; 
    
    [Header("Imágenes de Corazones (Opcional)")]
    public Image[] imagenesVidas; 
    public Sprite corazonLleno;
    public Sprite corazonVacio;
    
    private VidaJugador jugador;
    
    private void Start()
    {
        
        jugador = FindObjectOfType<VidaJugador>();
        
        if (jugador != null)
        {
            
            jugador.OnVidaCambiada.AddListener(ActualizarUIVidas);
            
            ActualizarUIVidas(jugador.GetVidasActuales());
        }
        else
        {
            Debug.LogError("No se encontró VidaJugador para la UI de vidas!");
        }
    }
    
    void ActualizarUIVidas(int vidasActuales)
    {
        
        int vidasMaximas = jugador != null ? jugador.GetVidaMaxima() : 3;
        
        
        string textoVida = $"Vidas: {vidasActuales}/{vidasMaximas}";
        
        if (textoVidas != null)
            textoVidas.text = textoVida;
            
        if (textoVidasTMP != null)
            textoVidasTMP.text = textoVida;
        
        
        ActualizarImagenesVidas(vidasActuales);
        
        Debug.Log($"UI Vidas actualizada: {textoVida}");
    }
    
    void ActualizarImagenesVidas(int vidasActuales)
    {
        if (imagenesVidas == null || imagenesVidas.Length == 0) return;
        
        for (int i = 0; i < imagenesVidas.Length; i++)
        {
            if (imagenesVidas[i] != null)
            {
                if (i < vidasActuales)
                {
                    // Vida disponible - corazón lleno
                    imagenesVidas[i].sprite = corazonLleno;
                    imagenesVidas[i].color = Color.white;
                }
                else
                {
                    // Vida perdida - corazón vacío
                    imagenesVidas[i].sprite = corazonVacio;
                    imagenesVidas[i].color = Color.gray;
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        
        if (jugador != null)
        {
            jugador.OnVidaCambiada.RemoveListener(ActualizarUIVidas);
        }
    }
}