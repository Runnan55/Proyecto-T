using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarridoAttack : MonoBehaviour
{
    public float sweepDuration = 1f;
    public float delayBetweenSwipes = 1f;
    public int totalSwipes = 3;
    public float damage = 50f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void ExecuteBarrido(Transform startPoint, Transform endPoint)
    {
        StartCoroutine(DoBarrido(startPoint.position, endPoint.position));
    }

    private IEnumerator DoBarrido(Vector3 originalStart, Vector3 originalEnd)
    {
        Vector3 direction = (originalEnd - originalStart).normalized;
        float distance = Vector3.Distance(originalStart, originalEnd);

        Vector3 toPlayer = player.position - originalStart;
        float projection = Vector3.Dot(toPlayer, direction);
        projection = Mathf.Clamp(projection, 0f, distance);

        Vector3 adjustedStart = originalStart + direction * (projection - distance / 2f);
        Vector3 adjustedEnd = adjustedStart + direction * distance;

        for (int i = 0; i < totalSwipes; i++)
        {
            GameObject barrido = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrido.transform.position = adjustedStart + Vector3.up;
            barrido.transform.localScale = new Vector3(2, 1, 15);
            barrido.GetComponent<Renderer>().material.color = Color.red;

            // Quitar el collider por defecto y poner BoxCollider como trigger
            Destroy(barrido.GetComponent<Collider>());
            BoxCollider bc = barrido.AddComponent<BoxCollider>();
            bc.isTrigger = true;

            // Agregar script de daño
            BarridoDamage bd = barrido.AddComponent<BarridoDamage>();
            bd.damage = damage;

            float elapsed = 0f;

            while (elapsed < sweepDuration)
            {
                barrido.transform.position = Vector3.Lerp(adjustedStart, adjustedEnd, elapsed / sweepDuration) + Vector3.up;
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(barrido);
            yield return new WaitForSeconds(delayBetweenSwipes);

            // Invertir dirección
            var temp = adjustedStart;
            adjustedStart = adjustedEnd;
            adjustedEnd = temp;
        }
    }
}