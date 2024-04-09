using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemySpawn
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
}

[System.Serializable]
public class Wave
{
    public EnemySpawn[] enemySpawns;
}

public class WaveManager : MonoBehaviour
{
    public Wave[] waves;

    private int currentWave = 0;

    public void StartNextWave()
    {
        if (currentWave < waves.Length)
        {
            StartCoroutine(SpawnWave(waves[currentWave]));
            currentWave++;
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.enemySpawns.Length; i++)
        {
            SpawnEnemy(wave.enemySpawns[i].enemyPrefab, wave.enemySpawns[i].spawnPoint);
            yield return new WaitForSeconds(1);
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint)
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}