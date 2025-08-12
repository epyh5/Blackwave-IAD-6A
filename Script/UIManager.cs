using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Textos UI")]
    public TextMeshProUGUI textoCuras;
    public TextMeshProUGUI textoNPCs;
    public TextMeshProUGUI textoProgreso;
    public TextMeshProUGUI textoObjetivo;
    public TextMeshProUGUI textoInstrucciones;
    
    [Header("Debug")]
    public bool mostrarDebug = true;
    
    void Start()
    {
        
        GameManager.OnCurasChanged += ActualizarContadorCuras;
        GameManager.OnNPCsChanged += ActualizarContadorNPCs;
        
        
        StartCoroutine(ActualizacionInicial());
        
        
        MostrarInstrucciones();
    }
    
    private System.Collections.IEnumerator ActualizacionInicial()
    {
        
        while (GameManager.Instance == null)
        {
            yield return null;
        }
        
        
        ActualizarContadorCuras(GameManager.Instance.GetCantidadCuras());
        ActualizarContadorNPCs(GameManager.Instance.GetNPCsCurados(), GameManager.Instance.GetNPCsTotales());
    }
    
    void OnDestroy()
    {
        
        GameManager.OnCurasChanged -= ActualizarContadorCuras;
        GameManager.OnNPCsChanged -= ActualizarContadorNPCs;
    }
    
    void ActualizarContadorCuras(int cantidad)
    {
        if (textoCuras != null)
        {
            textoCuras.text = $"Curas: {cantidad}";
        }
        
        ActualizarTextoObjetivo(cantidad);
        
        if (mostrarDebug)
            Debug.Log($"UI Curas actualizada: {cantidad}");
    }
    
    void ActualizarContadorNPCs(int curados, int totales)
    {
        if (textoNPCs != null)
        {
            textoNPCs.text = $"NPCs Curados: {curados}/{totales}";
        }
        
        if (textoProgreso != null)
        {
            if (totales > 0)
            {
                float porcentaje = (float)curados / totales * 100f;
                textoProgreso.text = $"Progreso: {porcentaje:F0}%";
                
                if (curados >= totales)
                {
                    textoProgreso.text += " ¡COMPLETADO!";
                    textoProgreso.color = Color.green;
                }
                else
                {
                    textoProgreso.color = Color.white;
                }
            }
            else
            {
                textoProgreso.text = "Progreso: 0%";
                textoProgreso.color = Color.white;
            }
        }
        
        if (mostrarDebug)
            Debug.Log($"UI NPCs actualizada: {curados}/{totales}");
    }
    
    void ActualizarTextoObjetivo(int curas)
    {
        if (textoObjetivo == null) return;
        
        string escenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (escenaActual.Contains("Nivel1") || escenaActual.Contains("1"))
        {
            textoObjetivo.text = $"Objetivo: Recolecta curas para el siguiente nivel";
        }
        else if (escenaActual.Contains("Nivel2") || escenaActual.Contains("2"))
        {
            if (GameManager.Instance != null)
            {
                int npcsTotal = GameManager.Instance.GetNPCsTotales();
                if (npcsTotal > 0)
                {
                    int npcsPorCurar = npcsTotal - GameManager.Instance.GetNPCsCurados();
                    int puedesCurar = Mathf.Min(curas, npcsPorCurar);
                    textoObjetivo.text = $"Puedes curar: {puedesCurar} NPCs";
                }
                else
                {
                    textoObjetivo.text = "Buscando NPCs enfermos...";
                }
            }
        }
        else
        {
            textoObjetivo.text = $"Curas disponibles: {curas}";
        }
    }
    
    void MostrarInstrucciones()
    {
        if (textoInstrucciones == null) return;
        
        string escenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (escenaActual.Contains("Nivel1") || escenaActual.Contains("1"))
        {
            textoInstrucciones.text = "Camina sobre las curas para recolectarlas";
        }
        else if (escenaActual.Contains("Nivel2") || escenaActual.Contains("2"))
        {
            textoInstrucciones.text = "Acércate a los NPCs enfermos y presiona C para curarlos";
        }
        else
        {
            textoInstrucciones.text = "¡Explora y completa los objetivos!";
        }
    }
}