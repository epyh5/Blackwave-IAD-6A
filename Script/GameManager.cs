using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Contador de Curas")]
    public int curasRecolectadas = 0;
    public int curasUsadas = 0;
    
    [Header("Contador de NPCs")]
    public int npcsTotales = 0;
    public int npcsCurados = 0;
    
    [Header("Debug")]
    public bool mostrarDebug = true;
    
    // Eventos para la UI
    public static System.Action<int> OnCurasChanged;
    public static System.Action<int, int> OnNPCsChanged;
    
    void Awake()
    {
       
        if (Instance != null && Instance != this)
        {
            if (mostrarDebug)
                Debug.Log("Destruyendo GameManager duplicado");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (mostrarDebug)
            Debug.Log("=== GAMEMANAGER CREADO Y PERSISTENTE ===");
    }
    
    void Start()
    {
        if (mostrarDebug)
            Debug.Log($"=== GAMEMANAGER START - Curas: {curasRecolectadas} ===");
        
        // Actualizar UI inicial
        ActualizarUI();
        
        // Contar NPCs después de un frame
        StartCoroutine(ContarNPCsConDelay());
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mostrarDebug)
            Debug.Log($"=== ESCENA CARGADA: {scene.name} - Curas: {curasRecolectadas} ===");
        
        // Resetear contadores de NPCs para la nueva escena
        npcsCurados = 0;
        npcsTotales = 0;
        
        // Contar NPCs después de que todo esté cargado
        StartCoroutine(ContarNPCsConDelay());
    }
    
    private System.Collections.IEnumerator ContarNPCsConDelay()
    {
        
        yield return null;
        yield return null;
        
        ContarNPCsEnEscena();
        ActualizarUI();
    }
    
    public void RecolectarCura()
    {
        curasRecolectadas++;
        
        if (mostrarDebug)
            Debug.Log($"=== CURA RECOLECTADA! Total: {curasRecolectadas} ===");
        
        ActualizarUI();
    }
    
    public bool UsarCura()
    {
        if (curasRecolectadas > 0)
        {
            curasRecolectadas--;
            curasUsadas++;
            
            if (mostrarDebug)
                Debug.Log($"=== CURA USADA! Restantes: {curasRecolectadas} ===");
            
            ActualizarUI();
            return true;
        }
        else
        {
            if (mostrarDebug)
                Debug.Log("=== NO TIENES CURAS! ===");
            return false;
        }
    }
    
    public void NPCCurado()
    {
        npcsCurados++;
        
        if (mostrarDebug)
            Debug.Log($"=== NPC CURADO! {npcsCurados}/{npcsTotales} ===");
        
        ActualizarUI();
        
        // Verificar si todos están curados
        if (npcsCurados >= npcsTotales && npcsTotales > 0)
        {
            if (mostrarDebug)
                Debug.Log("=== ¡TODOS LOS NPCs CURADOS! ¡NIVEL COMPLETADO! ===");
        }
    }
    
    void ContarNPCsEnEscena()
    {
        NPCEnfermo[] npcs = FindObjectsOfType<NPCEnfermo>();
        
        npcsTotales = npcs.Length;
        npcsCurados = 0;
        
        foreach (NPCEnfermo npc in npcs)
        {
            if (npc != null && npc.estaCurado)
                npcsCurados++;
        }
        
        if (mostrarDebug)
            Debug.Log($"=== NPCs ENCONTRADOS: {npcsCurados}/{npcsTotales} ===");
    }
    
    public void RecontarNPCs()
    {
        ContarNPCsEnEscena();
        ActualizarUI();
    }
    
    private void ActualizarUI()
    {
        OnCurasChanged?.Invoke(curasRecolectadas);
        OnNPCsChanged?.Invoke(npcsCurados, npcsTotales);
    }
    
    
    public int GetCantidadCuras() => curasRecolectadas;
    public int GetCurasUsadas() => curasUsadas;
    public bool TieneCuras() => curasRecolectadas > 0;
    public int GetNPCsCurados() => npcsCurados;
    public int GetNPCsTotales() => npcsTotales;
    public bool TodosNPCsCurados() => npcsCurados >= npcsTotales && npcsTotales > 0;
    

    [System.Obsolete("Solo para debug")]
    [ContextMenu("Debug: Añadir Cura")]
    void DebugAñadirCura()
    {
        RecolectarCura();
    }
    
    [System.Obsolete("Solo para debug")]
    [ContextMenu("Debug: Mostrar Estado")]
    void DebugMostrarEstado()
    {
        Debug.Log($"=== ESTADO ACTUAL ===\nCuras: {curasRecolectadas}\nNPCs: {npcsCurados}/{npcsTotales}\nEscena: {SceneManager.GetActiveScene().name}");
    }
}