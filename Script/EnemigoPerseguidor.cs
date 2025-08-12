using UnityEngine;

public class EnemigoPerseguidor : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento = 1f; 
    [SerializeField] private float distanciaDeteccion = 5f;
    [SerializeField] private float distanciaAtaque = 5f; 
    
    private Transform jugador;
    private bool persiguiendo = false;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (jugador == null) return;

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= distanciaDeteccion)
        {
            persiguiendo = true;
        }

        if (persiguiendo && distanciaAlJugador > distanciaAtaque)
        {
            PerseguirJugador();
        }
        else if (distanciaAlJugador > distanciaDeteccion * 1.5f)
        {
            persiguiendo = false;
        }
    }

    void PerseguirJugador()
    {
        Vector2 direccion = (jugador.position - transform.position).normalized;
        
        transform.Translate(direccion * velocidadMovimiento * Time.deltaTime);
        
        
        if (direccion.x > 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else if (direccion.x < 0)
            GetComponent<SpriteRenderer>().flipX = true;
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            VidaJugador vidaJugador = collision.gameObject.GetComponent<VidaJugador>();
            if (vidaJugador != null)
            {
                vidaJugador.RecibirDaño(1);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}