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
        // Calculamos el vector del barrido original
        Vector3 direction = (originalEnd - originalStart).normalized;
        float distance = Vector3.Distance(originalStart, originalEnd);

        // Proyectamos la posición del jugador en esa línea
        Vector3 toPlayer = player.position - originalStart;
        float projection = Vector3.Dot(toPlayer, direction);
        projection = Mathf.Clamp(projection, 0f, distance); // aseguramos que no salga de la línea

        // Nuevo punto de inicio: retrocedemos para que pase por el jugador
        Vector3 adjustedStart = originalStart + direction * (projection - distance / 2f);
        Vector3 adjustedEnd = adjustedStart + direction * distance;

        for (int i = 0; i < totalSwipes; i++)
        {
            GameObject barrido = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrido.transform.position = adjustedStart + Vector3.up;
            barrido.transform.localScale = new Vector3(2, 1, 15);
            barrido.GetComponent<Renderer>().material.color = Color.red;

            float elapsed = 0f;

            while (elapsed < sweepDuration)
            {
                barrido.transform.position = Vector3.Lerp(adjustedStart, adjustedEnd, elapsed / sweepDuration) + Vector3.up;
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(barrido);
            yield return new WaitForSeconds(delayBetweenSwipes);

            // Invertimos la dirección para el siguiente swipe
            var temp = adjustedStart;
            adjustedStart = adjustedEnd;
            adjustedEnd = temp;
        }
    }
}