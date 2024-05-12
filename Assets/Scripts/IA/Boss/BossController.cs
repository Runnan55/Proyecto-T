using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform[] attackPoints; // Posiciones para el patrón 1
    public GameObject fireballPrefab;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // Posiciones para el patrón 3
    public GameObject player;
    public GameObject particlePrefab;

    public float damageRadius;
    public float timeBetweenPatterns = 2f; // Tiempo entre cada repetición de patrones
    public LayerMask playerLayer;
    private float attackDuration = 20f;
    private int currentPattern = 0;


    void Start()
    {

        StartCoroutine(PatternRoutine());
        Pattern1();
    }

    IEnumerator PatternRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackDuration);
            currentPattern = (currentPattern + 1) % 3;
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
        StartCoroutine(Pattern1Routine());
    }


    void Pattern2()
    {
        Vector3 targetDirection = (player.transform.position - transform.position).normalized;
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        fireball.GetComponent<Rigidbody>().velocity = targetDirection * 10f; // Ajustar la velocidad según necesites
    }

    void Pattern3()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
    IEnumerator Pattern1Routine()
    {
        while (true)
        {
            Debug.Log("Starting Pattern 1");
            StartCoroutine(PreviewAndExecutePattern1()); // Inicia la previsualización y el ataque
            yield return new WaitForSeconds(timeBetweenPatterns); // Espera antes de pasar al siguiente patrón
        }
    }

    IEnumerator PreviewAndExecutePattern1()
    {
        // Previsualización del ataque 1 segundo antes
        Debug.Log("Previewing Pattern 1");
        yield return new WaitForSeconds(1f);

        // Ejecuta el ataque
        Debug.Log("Executing Pattern 1");
        foreach (var point in attackPoints)
        {
            Collider[] hitColliders = Physics.OverlapSphere(point.position, damageRadius, playerLayer);

            foreach (Collider collider in hitColliders)
            {
                // Comprobar si el objeto colisionado es el jugador
                if (collider.gameObject == player)
                {
                    // Aplicar daño al jugador
                    Debug.Log("Player hit by boss!");

                    // Acceder al componente Life del jugador
                    BossHealth playerLife = collider.gameObject.GetComponent<BossHealth>();
                    if (playerLife != null)
                    {
                        // Reducir la vida del jugador (tiempo)
                        playerLife.TakeDamage(60); // Cambia el valor de -60 según sea necesario
                    }
                }
            }
        }
    }
}