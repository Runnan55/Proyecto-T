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

    public void ExecutePalmada(Transform pointA, Transform pointB)
    {
        StartCoroutine(DoPalmada(pointA.position));
        StartCoroutine(DoPalmada(pointB.position));
    }

    private IEnumerator DoPalmada(Vector3 position)
    {
        GameObject sombra = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        sombra.transform.position = position;
        sombra.transform.localScale = new Vector3(shockwaveRadius, 0.05f, shockwaveRadius);
        sombra.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0.4f);

        yield return new WaitForSeconds(delayBeforeImpact);

        GameObject onda = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        onda.transform.position = position;
        onda.transform.localScale = Vector3.one * 0.1f;
        onda.GetComponent<Renderer>().material.color = new Color(1, 0.5f, 0, 0.5f);
        Destroy(onda, 1f);

        onda.AddComponent<ShockwaveScaler>();

        float dist = Vector3.Distance(player.position, position);
        if (dist <= shockwaveRadius)
        {
            Debug.Log("Jugador golpeado por palmada");
        }
        else
        {
            Debug.Log("Jugador fuera del área");
        }

        Destroy(sombra);
    }
}