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

    private float attackDuration = 20f;
    private int currentPattern = 0;

    void Start()
    {
        StartCoroutine(PatternRoutine());
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
                    StartCoroutine(Pattern1());
                    Debug.Log("pat1");
                    break;
                case 1:
                    StartCoroutine(Pattern2());
                    Debug.Log("pat2");

                    break;
                case 2:
                    StartCoroutine(Pattern3());
                    Debug.Log("pat3");

                    break;
            }
        }
    }

    IEnumerator Pattern1()
    {
        foreach (var point in attackPoints)
        {
            // Previsualización del ataque
            // Podrías usar un efecto visual aquí
            yield return new WaitForSeconds(1);
            // Implementar el ataque real
        }
    }

    IEnumerator Pattern2()
    {
        Vector3 targetDirection = (player.transform.position - transform.position).normalized;
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        fireball.GetComponent<Rigidbody>().velocity = targetDirection * 10f; // Ajustar la velocidad según necesites
        yield return new WaitForSeconds(1);
    }

    IEnumerator Pattern3()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
        yield return null;
    }
}