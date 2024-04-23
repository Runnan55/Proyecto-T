using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGen : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo que quieres spawnear
    public float timeBetweenWaves = 10f; // Tiempo entre oleadas de enemigos
    public float timeBetweenEnemies = 1f; // Tiempo entre la aparición de cada enemigo
    public int enemiesPerWave = 5; // Número inicial de enemigos por oleada
    public int incrementPerWave = 2; // Incremento de enemigos por oleada
    public float spawnDuration = 90f; // Duración total de spawn en segundos (1:30 min)
    public float timeBeforeOpen = 120f; // Tiempo antes de activar la variable open en segundos (2 min)
    private int currentWave = 0;
    private bool spawning = true; // Variable para controlar si se sigue spawnendo enemigos
    private bool isOpen = false; // Variable que se activa después de cierto tiempo

    void Start()
    {
        StartCoroutine(SpawnWaves());
        StartCoroutine(CheckTime());
    }

    IEnumerator SpawnWaves()
    {
        while (spawning)
        {
            // Espera el tiempo entre oleadas
            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave++;
            int numEnemiesThisWave = enemiesPerWave + incrementPerWave * (currentWave - 1);

            // Spawnea los enemigos de la oleada actual
            for (int i = 0; i < numEnemiesThisWave; i++)
            {
                SpawnEnemy();
                // Espera el tiempo entre enemigos
                yield return new WaitForSeconds(timeBetweenEnemies);
            }
        }
    }

    IEnumerator CheckTime()
    {
        yield return new WaitForSeconds(spawnDuration);

        // Después de 1:30 min, dejamos de spawnear
        spawning = false;

        // Esperamos otros 30 segundos antes de activar la variable isOpen
        yield return new WaitForSeconds(timeBeforeOpen - spawnDuration);

        // Activamos la variable isOpen
        isOpen = true;
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }
}