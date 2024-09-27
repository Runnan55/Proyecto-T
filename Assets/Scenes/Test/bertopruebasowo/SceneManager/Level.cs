using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawner
{
    public Enemy enemy;
    public Transform spawnPoint;
    public float spawnDelay; // Nuevo campo para el delay individual
}

[System.Serializable]
public class EnemyWave
{
    public List<EnemySpawner> enemySpawns;
}

public class Level : MonoBehaviour
{
    [Header("Doors config")]
    public Door entranceDoor;
    public Door exitDoor;

    [Header("Waves config")]
    public List<EnemyWave> waves;

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
            Enemy newEnemy = Instantiate(enemySpawner.enemy, enemySpawner.spawnPoint.position, Quaternion.identity);
            
            // Almacena la escala y la rotación originales
            Vector3 originalScale = newEnemy.transform.localScale;
            Quaternion originalRotation = newEnemy.transform.rotation;

            // Cambia el padre
            newEnemy.transform.SetParent(this.transform, true);

            // Espera un frame
            yield return null;

            // Restablece la escala y la rotación originales
            newEnemy.transform.localScale = originalScale;
            newEnemy.transform.rotation = originalRotation;

            newEnemy.level = this; // Asegúrate de que el enemigo sepa a qué instancia de Level debe referirse
            yield return new WaitForSeconds(enemySpawner.spawnDelay); // Usa el delay individual
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