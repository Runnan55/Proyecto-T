using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawner
{
    public Enemy enemy;
    public Transform spawnPoint;
}

[System.Serializable]
public class EnemyWave
{
    public List<EnemySpawner> enemySpawns;
}

public class Level : MonoBehaviour
{
    public List<EnemyWave> waves;
    public Door entranceDoor, exitDoor;
    public float delayBetweenEnemies = 1f;
    private int currentWave = 0;
    private int defeatedEnemies = 0;
    private bool hasPlayerEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayerEntered)
        {
            StartCoroutine(entranceDoor.Close());
            Debug.Log(hasPlayerEntered);
            StartNextWave();
            hasPlayerEntered = true; 
            Debug.Log(hasPlayerEntered);
        }
    }

    public void StartNextWave()
    {
        if (currentWave < waves.Count)
        {
            defeatedEnemies = waves[currentWave].enemySpawns.Count;
            StartCoroutine(SpawnWave(waves[currentWave]));
            currentWave++;
        }
    }

    private IEnumerator SpawnWave(EnemyWave wave)
    {
        foreach (EnemySpawner enemySpawner in wave.enemySpawns)
        {
            Instantiate(enemySpawner.enemy, enemySpawner.spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(delayBetweenEnemies);
        }
    }

    public void EnemyDefeated(Enemy enemy)
    {
        Debug.Log("Enemy defeated");
        defeatedEnemies--;
        Debug.Log("Defeated enemies: " + defeatedEnemies);

        if (defeatedEnemies <= 0)
        {
            if (currentWave == waves.Count)
            {
                StartCoroutine(entranceDoor.Open());
                StartCoroutine(exitDoor.Open());
            }
            else
            {
                StartNextWave();
            }
        }
    }
}