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

    public void ExecutePalmadaSobreJugador()
    {
        Vector3 posicionJugador = player.position;
        Vector3 offset = new Vector3(1.5f, 0, 0); // separación izquierda/derecha

        StartCoroutine(DoPalmada(posicionJugador - offset)); // izquierda
        StartCoroutine(DoPalmada(posicionJugador + offset)); // derecha
    }

    public void ExecutePalmada(Transform pointA, Transform pointB)
    {
        StartCoroutine(DoPalmada(pointA.position));
        StartCoroutine(DoPalmada(pointB.position));
    }

    private IEnumerator DoPalmada(Vector3 position)
    {
        // Crear sombra visual
        GameObject sombra = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        sombra.transform.position = position;
        sombra.transform.localScale = new Vector3(shockwaveRadius, 0.05f, shockwaveRadius);
        Destroy(sombra.GetComponent<Collider>());
        sombra.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0.4f);

        yield return new WaitForSeconds(delayBeforeImpact);

        // Crear la onda expansiva
        GameObject onda = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        onda.transform.position = position;
        onda.transform.localScale = Vector3.one * 0.3f;
        onda.GetComponent<Renderer>().material.color = new Color(1, 0.5f, 0, 0.5f);

        // Reemplazar el collider por uno trigger
        Destroy(onda.GetComponent<Collider>());
        SphereCollider sc = onda.AddComponent<SphereCollider>();
        sc.isTrigger = true;

        // Añadir el script de escalado y daño
        ShockwaveScaler scaler = onda.AddComponent<ShockwaveScaler>();
        scaler.maxScale = shockwaveRadius * 2f;
        scaler.scaleSpeed = 5f;
        scaler.damage = damage;

        Destroy(onda, 3f);
        Destroy(sombra);
    }

    public void ExecuteDoblePalmadaSobreJugador()
    {
        StartCoroutine(DoDoublePalmadaTrackMovement());
    }

    private IEnumerator DoDoublePalmadaTrackMovement()
    {
        Vector3 offset = new Vector3(1.5f, 0, 0);
        Vector3 pos1 = player.position - offset;
        StartCoroutine(DoPalmada(pos1));

        yield return new WaitForSeconds(1f);

        Vector3 pos2 = player.position + offset;
        StartCoroutine(DoPalmada(pos2));
    }
}