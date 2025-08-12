using UnityEngine;

public class EnemigoS : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 2f;
    public float distanciaCaminata = 3f;
    public bool mostrarDebug = true;

    [Header("Muerte del Jugador")]
    public bool mataAlJugador = true;
    public int dañoPorContacto = 1;
    public float tiempoEntreDaños = 1f;

    private Vector3 puntoInicial;
    private Vector3 objetivo;
    private bool moviendoDerecha = true;
    private float ultimoDaño = 0f;

    private void Start()
    {
        puntoInicial = transform.position;
        ActualizarObjetivo();
    }

    private void Update()
    {
        MoverEnemigo();
    }

    private void MoverEnemigo()
    {
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, objetivo) < 0.1f)
        {
            moviendoDerecha = !moviendoDerecha;
            ActualizarObjetivo();
        }

        // Voltear sprite según dirección
        Vector3 escala = transform.localScale;
        escala.x = moviendoDerecha ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    private void ActualizarObjetivo()
    {
        if (moviendoDerecha)
            objetivo = puntoInicial + Vector3.right * distanciaCaminata;
        else
            objetivo = puntoInicial + Vector3.left * distanciaCaminata;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (mostrarDebug)
            Debug.Log($"[ENEMIGO DEBUG] Trigger detectado con: {other.gameObject.name}, Tag: '{other.tag}'");

        if (other.CompareTag("Player"))
        {
            if (mostrarDebug)
                Debug.Log("[ENEMIGO DEBUG] Confirmado: Es el jugador!");
            
            DañarJugador(other.gameObject);
        }
        else
        {
            if (mostrarDebug)
                Debug.Log($"[ENEMIGO DEBUG] No es el jugador, tag incorrecto: '{other.tag}'");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mostrarDebug)
            Debug.Log($"[ENEMIGO DEBUG] Colisión detectada con: {collision.gameObject.name}, Tag: '{collision.gameObject.tag}'");

        if (collision.gameObject.CompareTag("Player"))
        {
            if (mostrarDebug)
                Debug.Log("[ENEMIGO DEBUG] Confirmado: Es el jugador por colisión!");
            
            DañarJugador(collision.gameObject);
        }
    }

    private void DañarJugador(GameObject jugadorObj)
    {
        
        if (Time.time - ultimoDaño < tiempoEntreDaños)
        {
            if (mostrarDebug)
                Debug.Log("[ENEMIGO DEBUG] Muy poco tiempo desde el último daño, ignorando...");
            return;
        }

        if (mostrarDebug)
            Debug.Log($"[ENEMIGO DEBUG] Intentando dañar a: {jugadorObj.name}");

        
        VidaJugador vidaJugador = jugadorObj.GetComponent<VidaJugador>();
        MovimientoJugador movimientoJugador = jugadorObj.GetComponent<MovimientoJugador>();

        if (mostrarDebug)
        {
            Debug.Log($"[ENEMIGO DEBUG] VidaJugador encontrado: {vidaJugador != null}");
            Debug.Log($"[ENEMIGO DEBUG] MovimientoJugador encontrado: {movimientoJugador != null}");
        }

        if (vidaJugador != null)
        {
            if (mostrarDebug)
                Debug.Log($"[ENEMIGO DEBUG] Aplicando {dañoPorContacto} de daño via VidaJugador");
            
            vidaJugador.RecibirDaño(dañoPorContacto);
            ultimoDaño = Time.time;
        }
        else if (movimientoJugador != null)
        {
            if (mostrarDebug)
                Debug.Log($"[ENEMIGO DEBUG] VidaJugador no encontrado, usando MovimientoJugador como backup");
            
            movimientoJugador.RecibirDaño(dañoPorContacto);
            ultimoDaño = Time.time;
        }
        else
        {
            if (mostrarDebug)
                Debug.Log("[ENEMIGO DEBUG] ERROR: No se encontró ningún script de vida en el jugador!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (mostrarDebug)
        {
            Gizmos.color = Color.red;
            Vector3 inicio = puntoInicial;
            Vector3 fin = puntoInicial + Vector3.right * distanciaCaminata;
            Vector3 fin2 = puntoInicial + Vector3.left * distanciaCaminata;
            
            Gizmos.DrawLine(inicio, fin);
            Gizmos.DrawLine(inicio, fin2);
            Gizmos.DrawWireSphere(fin, 0.2f);
            Gizmos.DrawWireSphere(fin2, 0.2f);
        }
    }
}