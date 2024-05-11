using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform[] attackPoints; // Posiciones para el patr�n 1
    public GameObject fireballPrefab;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // Posiciones para el patr�n 3
    public GameObject player;
    public GameObject particlePrefab;
   
    public float damageRadius;
    public float timeBetweenPatterns = 2f; // Tiempo entre cada repetici�n de patrones
    public LayerMask playerLayer;
    private float attackDuration = 20f;
    private int currentPattern = 0;
    private Life playerLife;


    void Start()
    {
        StartCoroutine(PatternRoutine());
        playerLife = FindObjectOfType<Life>();

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
        foreach (var point in attackPoints)
        {
            StartCoroutine(AttackPlayer(point.position));
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

    IEnumerator AttackPlayer(Vector3 position)
    {
        // Activar el efecto visual (sistema de part�culas)
        GameObject particleEffect = Instantiate(particlePrefab, position, Quaternion.identity);
        yield return new WaitForSeconds(1); // Duraci�n de la previsualizaci�n
        Destroy(particleEffect); // Desactivar el efecto visual

        // Detectar y aplicar da�o a los jugadores dentro del radio de da�o
        Collider[] hitColliders = Physics.OverlapSphere(position, damageRadius, playerLayer);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                // Restar vida al jugador utilizando el script Life si est� presente
                Life playerLife = collider.GetComponent<Life>();
                if (playerLife != null)
                {
                    playerLife.ModifyTime(-damageRadius); // Ajusta el valor seg�n sea necesario
                }
            }
        }
    }
}