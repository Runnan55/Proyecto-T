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

    public int EnemiesRemaining { get; private set; }

    public void StartNextWave()
    {
        Debug.Log("Siguiente oleada");
        
        if (currentWave < waves.Length)
        {
            EnemiesRemaining = waves[currentWave].enemySpawns.Length;
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

    public void EnemyDied()
    {
        Debug.Log("Enemigo muerto para las oleadas");
        EnemiesRemaining--;
        if (EnemiesRemaining <= 0)
        {
            StartNextWave();
        }
    }

    #region DEBUG //    ***** DEBUG ***** 
    public void Update()
   {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartNextWave();
        }
    }
    #endregion DEBUG
}