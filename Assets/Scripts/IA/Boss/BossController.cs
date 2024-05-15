using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : Enemy
{
    public GameObject fireballPrefab;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // Posiciones para el patrón 3
    public GameObject player;
    public GameObject circlePrefab;
    public GameObject previewCirclePrefab; // Prefab para la previsualización
    public GameObject triggerArea;

    public float damageRadius;
    public float timeBetweenPatterns = 2f; // Tiempo entre cada repetición de patrones
    public LayerMask playerLayer;
    private float attackDuration = 20f;
    private int currentPattern = 0;
    public int numberOfCircles = 5; // Cantidad de círculos a generar
    public float previewDuration = 2f; // Duración de la previsualización

    void Start()
    {
        StartCoroutine(PatternRoutine());
    }

    IEnumerator PatternRoutine()
    {
        while (true)
        {
            switch (currentPattern)
            {
                case 0:
                    Debug.Log("pat1");
                    StartCoroutine(RepeatPattern(Pattern1, timeBetweenPatterns));
                    break;
                case 1:
                    Debug.Log("pat2");
                    StartCoroutine(RepeatPattern(Pattern2, 0.5f));
                    break;
                case 2:
                    Debug.Log("pat3");
                    StartCoroutine(RepeatPattern(Pattern3, timeBetweenPatterns));
                    break;
            }
            yield return new WaitForSeconds(attackDuration);
            currentPattern = (currentPattern + 1) % 3;
        }
    }

    IEnumerator RepeatPattern(System.Action pattern, float timeBetweenRepetitions)
    {
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            pattern.Invoke();
            elapsedTime += timeBetweenRepetitions;
            yield return new WaitForSeconds(timeBetweenRepetitions);
        }
    }

    void Pattern1()
    {
        if (triggerArea != null)
        {
            Collider triggerCollider = triggerArea.GetComponent<Collider>();
            if (triggerCollider != null && triggerCollider.isTrigger)
            {
                // Obtén los límites del collider del triggerArea
                Vector3 minPosition = triggerCollider.bounds.min;
                Vector3 maxPosition = triggerCollider.bounds.max;

                // Itera para generar la cantidad de círculos especificada
                for (int i = 0; i < numberOfCircles; i++)
                {
                    // Genera una posición aleatoria dentro del rango definido por los límites del collider
                    Vector3 randomPosition = new Vector3(Random.Range(minPosition.x, maxPosition.x),
                                                         Random.Range(minPosition.y, maxPosition.y),
                                                         Random.Range(minPosition.z, maxPosition.z));

                    // Inicia la rutina para la previsualización y la instancia final
                    StartCoroutine(ShowPreviewAndInstantiate(randomPosition));
                }
            }
        }
    }

    IEnumerator ShowPreviewAndInstantiate(Vector3 position)
    {
        // Instancia el objeto de previsualización
        GameObject previewCircle = Instantiate(previewCirclePrefab, position, Quaternion.identity);

        // Espera el tiempo de previsualización
        yield return new WaitForSeconds(previewDuration);

        // Destruye el objeto de previsualización
        Destroy(previewCircle);

        // Instancia el círculo final
        GameObject circle = Instantiate(circlePrefab, position, Quaternion.identity);

        // Destruye el círculo después de 2 segundos
        Destroy(circle, 2f);
    }

    void Pattern2()
    {
        Vector3 targetDirection = (player.transform.position - transform.position).normalized;
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        fireball.GetComponent<Rigidbody>().velocity = targetDirection * 20f; // Ajustar la velocidad según necesites
    }

    void Pattern3()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}