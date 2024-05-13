using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject fireballPrefab;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // Posiciones para el patr�n 3
    public GameObject player;
    public GameObject circlePrefab;
    public GameObject triggerArea;

    public float damageRadius;
    public float timeBetweenPatterns = 2f; // Tiempo entre cada repetici�n de patrones
    public LayerMask playerLayer;
    private float attackDuration = 20f;
    private int currentPattern = 0;
    public int numberOfCircles = 5; // Cantidad de c�rculos a generar



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
                    StartCoroutine(RepeatPattern(Pattern2, timeBetweenPatterns));
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
                // Obt�n los l�mites del collider del triggerArea
                Vector3 minPosition = triggerCollider.bounds.min;
                Vector3 maxPosition = triggerCollider.bounds.max;

                // Itera para generar la cantidad de c�rculos especificada
                for (int i = 0; i < numberOfCircles; i++)
                {
                    // Genera una posici�n aleatoria dentro del rango definido por los l�mites del collider
                    Vector3 randomPosition = new Vector3(Random.Range(minPosition.x, maxPosition.x),
                                                         Random.Range(minPosition.y, maxPosition.y),
                                                         Random.Range(minPosition.z, maxPosition.z));

                    // Instancia un gameobject en la posici�n aleatoria
                    GameObject circle = Instantiate(circlePrefab, randomPosition, Quaternion.identity);
                    // Puedes ajustar la rotaci�n u otras propiedades aqu� si es necesario

                    // Destruye el c�rculo despu�s de 2 segundos
                    Destroy(circle, 2f);
                }
            }
            else
            {
                Debug.LogError("El GameObject asignado como triggerArea no tiene un Collider marcado como trigger.");
            }
        }
        else
        {
            Debug.LogError("No se ha asignado ning�n GameObject como triggerArea.");
        }
    }

    void Pattern2()
    {
        Vector3 targetDirection = (player.transform.position - transform.position).normalized;
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        fireball.GetComponent<Rigidbody>().velocity = targetDirection * 10f; // Ajustar la velocidad seg�n necesites
    }

    void Pattern3()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }


    
    

}