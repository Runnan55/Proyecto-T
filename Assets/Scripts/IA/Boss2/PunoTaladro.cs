using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunoTaladro : MonoBehaviour
{
    public float delayBeforeImpact = 3f;
    public float damageRadius = 2f;
    public int maxUses = 2;
    private int uses = 0;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void ExecutePunetazo(Transform pointA, Transform pointB)
    {
        if (uses >= maxUses) return;

        uses++;
        StartCoroutine(DoPunetazo(pointA.position));
        StartCoroutine(DoPunetazo(pointB.position));
    }

    private IEnumerator DoPunetazo(Vector3 position)
    {
        GameObject sombra = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        sombra.transform.position = position;
        sombra.transform.localScale = new Vector3(damageRadius, 0.05f, damageRadius);
        sombra.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0.3f);

        yield return new WaitForSeconds(delayBeforeImpact);

        GameObject hueco = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        hueco.transform.position = position;
        hueco.transform.localScale = new Vector3(damageRadius, 0.1f, damageRadius);
        hueco.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f);

        Debug.Log("Suelo perforado por puñetazo taladro");

        Destroy(sombra);
    }
}