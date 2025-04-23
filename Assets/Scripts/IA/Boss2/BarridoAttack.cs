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

    private IEnumerator DoBarrido(Vector3 from, Vector3 to)
    {
        for (int i = 0; i < totalSwipes; i++)
        {
            GameObject barrido = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrido.transform.position = from + Vector3.up;
            barrido.transform.localScale = new Vector3(2, 1, 5);
            barrido.GetComponent<Renderer>().material.color = Color.red;

            float elapsed = 0f;

            while (elapsed < sweepDuration)
            {
                barrido.transform.position = Vector3.Lerp(from, to, elapsed / sweepDuration) + Vector3.up;
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(barrido);
            yield return new WaitForSeconds(delayBetweenSwipes);

            var temp = from;
            from = to;
            to = temp;
        }
    }
}