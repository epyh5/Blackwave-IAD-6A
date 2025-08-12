using UnityEngine;
using System.Collections;

public class NPCEnfermo : MonoBehaviour
{
    [Header("Estado")]
    public bool estaCurado = false;
    public string nombreNPC = "Aldeano";

    [Header("Colores")]
    public Color colorEnfermo = Color.red;
    public Color colorSano = Color.green;

    [Header("Debug")]
    public bool mostrarDebug = true;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ActualizarApariencia();
    }

    public void CurarDirectamente()
    {
        if (!estaCurado)
        {
            CurarNPC();
        }
    }

    void CurarNPC()
    {
        estaCurado = true;
        ActualizarApariencia();

        if (mostrarDebug)
            Debug.Log($"¡{nombreNPC} ha sido curado!");

        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NPCCurado();
        }

        StartCoroutine(EfectoCuracion());
    }

    IEnumerator EfectoCuracion()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = colorSano;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void ActualizarApariencia()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = estaCurado ? colorSano : colorEnfermo;
        }
    }
}
