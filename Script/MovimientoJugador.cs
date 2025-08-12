using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento")]
    private Vector2 movimiento;
    private Rigidbody2D rb2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public float velocidadMovimiento = 7f;

    [Header("Salto")]
    public float fuerzaSalto = 14f;
    public bool puedeDoblesSalto = true;
    public float fuerzaDoblesSalto = 10f;
    public LayerMask capaSuelo = -1;
    public Transform verificadorSuelo;
    public float radioVerificacion = 0.2f;
    private bool enSuelo;
    private bool estabaNSuelo;
    private int saltosRestantes = 0;

    [Header("Coyote Time & Jump Buffer")]
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Sistema de Curación")]
    public float radioCuracion = 5f;
    
    [Header("Debug")]
    public bool mostrarDebug = true;

    private bool estabaSaltando = false;
    private VidaJugador sistemaVida; 

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (animator == null && mostrarDebug)
            Debug.Log("No se encontró Animator - las animaciones estarán deshabilitadas");
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        
        sistemaVida = GetComponent<VidaJugador>();
        if (sistemaVida == null && mostrarDebug)
            Debug.LogWarning("No se encontró VidaJugador - el sistema de vida no funcionará");

        rb2D.gravityScale = 1.5f;
        rb2D.linearDamping = 0f;
        rb2D.freezeRotation = true;

        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.sharedMaterial = CrearMaterialFisicoSuave();

        if (mostrarDebug)
            Debug.Log($"Rigidbody2D configurado - Gravity: {rb2D.gravityScale}, Jump Force: {fuerzaSalto}");
    }

    private PhysicsMaterial2D CrearMaterialFisicoSuave()
    {
        PhysicsMaterial2D smoothMaterial = new PhysicsMaterial2D("SmoothPlayer");
        smoothMaterial.friction = 0.3f;
        smoothMaterial.bounciness = 0f;
        return smoothMaterial;
    }

    private void Update()
    {
        estabaNSuelo = enSuelo;
        enSuelo = Physics2D.OverlapCircle(verificadorSuelo.position, radioVerificacion, capaSuelo);

        if (enSuelo && !estabaNSuelo)
        {
            saltosRestantes = puedeDoblesSalto ? 2 : 1;
            coyoteTimeCounter = coyoteTime;
        }
        else if (!enSuelo)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0 && (saltosRestantes > 0 || coyoteTimeCounter > 0))
        {
            RealizarSalto();
            jumpBufferCounter = 0;
        }

        ActualizarAnimaciones();

        if (mostrarDebug)
        {
            int vidaActual = sistemaVida != null ? sistemaVida.GetVidaActual() : 0;
            Debug.Log($"Saltos restantes: {saltosRestantes}, En suelo: {enSuelo}, Vida: {vidaActual}");
        }
    }

    private void FixedUpdate()
    {
        float targetVelocityX = movimiento.x * velocidadMovimiento;
        rb2D.linearVelocity = new Vector2(targetVelocityX, rb2D.linearVelocity.y);

        if (Mathf.Abs(movimiento.x) < 0.1f && enSuelo)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x * 0.85f, rb2D.linearVelocity.y);
        }
    }

    public void OnMover(InputAction.CallbackContext context)
    {
        Vector2 valorEntrada = context.ReadValue<Vector2>();

        if (context.performed)
        {
            movimiento = new Vector2(valorEntrada.x, 0);
            if (Mathf.Abs(movimiento.x) > 0.1f)
            {
                spriteRenderer.flipX = movimiento.x < 0;
            }
        }
        else if (context.canceled)
        {
            movimiento = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
            if (saltosRestantes > 0 || coyoteTimeCounter > 0)
            {
                RealizarSalto();
                jumpBufferCounter = 0;
            }
        }
    }

    
    public void OnCurar(InputAction.CallbackContext context)
    {
        Debug.Log($"OnCurar llamado - Fase: {context.phase}");
        
        if (context.performed)
        {
            Debug.Log("Input System detectó C correctamente!");
            if (mostrarDebug)
                Debug.Log("=== TECLA C PRESIONADA ===");

            CurarNPCs();
        }
    }

    
    private void CurarNPCs()
    {
        Debug.Log("Iniciando proceso de curación...");

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no encontrado!");
            return;
        }
        Debug.Log("GameManager encontrado");

        if (!GameManager.Instance.TieneCuras())
        {
            Debug.Log($"No tienes curas! Curas actuales: {GameManager.Instance.GetCantidadCuras()}");
            return;
        }
        Debug.Log($"Tienes {GameManager.Instance.GetCantidadCuras()} curas disponibles");

        NPCEnfermo[] npcs = FindObjectsOfType<NPCEnfermo>();
        Debug.Log($"Encontrados {npcs.Length} NPCs en la escena");

        if (npcs.Length == 0)
        {
            Debug.Log("No hay NPCs en la escena");
            return;
        }

        NPCEnfermo npcParaCurar = null;
        float distanciaMenor = float.MaxValue;

        foreach (NPCEnfermo npc in npcs)
        {
            if (npc == null) continue;

            float distancia = Vector2.Distance(transform.position, npc.transform.position);
            Debug.Log($"NPC '{npc.nombreNPC}': {distancia:F1}m de distancia, Curado: {npc.estaCurado}");

            if (distancia <= radioCuracion && !npc.estaCurado)
            {
                if (distancia < distanciaMenor)
                {
                    distanciaMenor = distancia;
                    npcParaCurar = npc;
                    Debug.Log($"Nuevo candidato: {npc.nombreNPC} a {distancia:F1}m");
                }
            }
        }

        if (npcParaCurar != null)
        {
            Debug.Log($"Intentando curar a {npcParaCurar.nombreNPC}...");
            
            if (GameManager.Instance.UsarCura())
            {
                npcParaCurar.CurarDirectamente();
                Debug.Log($"{npcParaCurar.nombreNPC} CURADO EXITOSAMENTE!");
                Debug.Log($"Curas restantes: {GameManager.Instance.GetCantidadCuras()}");
            }
            else
            {
                Debug.LogError("Error al usar la cura en GameManager");
            }
        }
        else
        {
            Debug.Log($"No hay NPCs enfermos en un radio de {radioCuracion}m");
            
            Debug.Log("NPCs cercanos (todos):");
            foreach (NPCEnfermo npc in npcs)
            {
                float dist = Vector2.Distance(transform.position, npc.transform.position);
                Debug.Log($"   - {npc.nombreNPC}: {dist:F1}m, Curado: {npc.estaCurado}");
            }
        }
    }

    private void RealizarSalto()
    {
        if (enSuelo || coyoteTimeCounter > 0)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, fuerzaSalto);
            saltosRestantes = puedeDoblesSalto ? 1 : 0;
            coyoteTimeCounter = 0;

            if (mostrarDebug)
                Debug.Log($"Primer salto! Fuerza: {fuerzaSalto}");
        }
        else if (saltosRestantes > 0 && puedeDoblesSalto)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, fuerzaDoblesSalto);
            saltosRestantes--;

            if (mostrarDebug)
                Debug.Log($"Doble salto! Fuerza: {fuerzaDoblesSalto}");
        }

        if (animator != null)
        {
            animator.SetBool("estaSaltando", true);
            estabaSaltando = true;
        }
    }

    private void ActualizarAnimaciones()
    {
        if (animator == null) return;

        float velocidadHorizontal = Mathf.Abs(rb2D.linearVelocity.x);
        float velocidadVertical = rb2D.linearVelocity.y;

        animator.SetBool("estaCaminando", velocidadHorizontal > 0.1f);
        animator.SetFloat("velocidad", velocidadHorizontal);
        animator.SetFloat("velocidadY", velocidadVertical);

        bool deberiaEstarSaltando = !enSuelo && velocidadVertical > -2f;

        if (deberiaEstarSaltando != estabaSaltando)
        {
            animator.SetBool("estaSaltando", deberiaEstarSaltando);
            estabaSaltando = deberiaEstarSaltando;
        }

        if (mostrarDebug)
        {
            Debug.Log($"VelX: {velocidadHorizontal:F2}, EstaCaminando: {velocidadHorizontal > 0.1f}, EstaSaltando: {deberiaEstarSaltando}");
        }
    }

    
    public void RecibirDaño(int cantidad = 1)
    {
        if (sistemaVida != null)
        {
            sistemaVida.RecibirDaño(cantidad);
        }
        else
        {
            
            if (mostrarDebug)
                Debug.Log("Sin VidaJugador - reiniciando escena directamente");
            
            Invoke(nameof(ReiniciarEscena), 1f);
        }
    }

    
    private void ReiniciarEscena()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1"); 
    }

    public void Curar(int cantidad = 1)
    {
        if (sistemaVida != null)
        {
            sistemaVida.Curar(cantidad);
        }
        else if (mostrarDebug)
        {
            Debug.Log($"Sin VidaJugador - curación ignorada");
        }
    }

    // MÉTODOS PARA MANEJAR ANIMACIONES DE DAÑO Y MUERTE
    public void ActivarAnimacionDaño()
    {
        if (animator != null)
        {
            animator.SetBool("estaDañada", true);
            // Desactivar después de un tiempo
            Invoke(nameof(DesactivarAnimacionDaño), 0.5f);
        }
    }

    public void ActivarAnimacionMuerte()
    {
        if (animator != null)
            animator.SetTrigger("morir");
    }

    private void DesactivarAnimacionDaño()
    {
        if (animator != null)
            animator.SetBool("estaDañada", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (verificadorSuelo != null)
        {
            Gizmos.color = enSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(verificadorSuelo.position, radioVerificacion);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(1.2f, 1.6f, 0));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radioCuracion);
        
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, radioCuracion);
    }
}