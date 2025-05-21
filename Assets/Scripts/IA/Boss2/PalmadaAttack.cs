using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmadaAttack : MonoBehaviour
{
    public float delayBeforeImpact = 3f;
    public float shockwaveRadius = 3f;
    public float damage = 50f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Llama esta función desde otro script para ejecutar la palmada donde está el jugador
    public void ExecutePalmadaSobreJugador()
    {
        Vector3 posicionJugador = player.position;
        StartCoroutine(DoPalmada(posicionJugador));
    }

    // Si aún quieres mantener la opción doble (por ejemplo, manos a izquierda y derecha del jugador)
    public void ExecutePalmada(Transform pointA, Transform pointB)
    {
        StartCoroutine(DoPalmada(pointA.position));
        StartCoroutine(DoPalmada(pointB.position));
    }

    private IEnumerator DoPalmada(Vector3 position)
    {
        // Crear sombra
        GameObject sombra = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        sombra.transform.position = position;
        sombra.transform.localScale = new Vector3(shockwaveRadius, 0.05f, shockwaveRadius);
        sombra.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0.4f);

        // Espera antes del impacto
        yield return new WaitForSeconds(delayBeforeImpact);

        // Crear onda expansiva visual
        GameObject onda = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        onda.transform.position = position;
        onda.transform.localScale = Vector3.one * 0.1f;
        onda.GetComponent<Renderer>().material.color = new Color(1, 0.5f, 0, 0.5f);
        Destroy(onda, 3f);

        onda.AddComponent<ShockwaveScaler>(); // Escala visualmente la onda

        // Comprobar daño
        float dist = Vector3.Distance(player.position, position);
        if (dist <= shockwaveRadius)
        {
            Debug.Log("Jugador golpeado por palmada");
            // Aquí iría tu sistema de daño
            // player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Jugador fuera del área");
        }

        // Eliminar sombra
        Destroy(sombra);
    }

    public void ExecuteDoblePalmadaSobreJugador()
    {
        StartCoroutine(DoDoublePalmadaTrackMovement());
    }

    private IEnumerator DoDoublePalmadaTrackMovement()
    {
        // 1. Guardamos la posición actual del jugador para la primera palmada
        Vector3 pos1 = player.position;
        StartCoroutine(DoPalmada(pos1));  // Primera palmada (donde estaba)

        // 2. Esperamos un segundo
        yield return new WaitForSeconds(1f);

        // 3. Capturamos la nueva posición actual del jugador
        Vector3 pos2 = player.position;
        StartCoroutine(DoPalmada(pos2));  // Segunda palmada (donde se ha movido)
    }
}