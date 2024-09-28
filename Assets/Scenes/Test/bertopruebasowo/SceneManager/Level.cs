using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawner
{
    public Enemy enemy;
    public Transform spawnPoint;
    public float spawnDelay;
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

    [Header("Warning config")]
    public GameObject warning;

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
    // Agrupamos los spawns por su spawnDelay
    Dictionary<float, List<EnemySpawner>> spawnsPorTiempo = new Dictionary<float, List<EnemySpawner>>();

    foreach (EnemySpawner enemySpawner in wave.enemySpawns)
    {
        if (!spawnsPorTiempo.ContainsKey(enemySpawner.spawnDelay))
        {
            spawnsPorTiempo[enemySpawner.spawnDelay] = new List<EnemySpawner>();
        }
        spawnsPorTiempo[enemySpawner.spawnDelay].Add(enemySpawner);
    }

    float tiempoDesdeUltimoSpawn = 0f;

    foreach (var grupoSpawn in spawnsPorTiempo)
    {
        float delayGrupo = grupoSpawn.Key;

        // Esperamos el tiempo entre el último spawn y el actual grupo de spawns
        yield return new WaitForSeconds(delayGrupo - tiempoDesdeUltimoSpawn);

        // Spawneamos todos los enemigos del grupo al mismo tiempo
        foreach (EnemySpawner enemySpawner in grupoSpawn.Value)
        {
            // Instanciar la advertencia
            GameObject warningInstance = Instantiate(warning, enemySpawner.spawnPoint.position, Quaternion.identity);
            warningInstance.transform.LookAt(Camera.main.transform);
            yield return new WaitForSeconds(0.1f);
            Destroy(warningInstance);

            // Instanciar el enemigo
            Enemy newEnemy = Instantiate(enemySpawner.enemy, enemySpawner.spawnPoint.position, Quaternion.identity);
            Vector3 escalaOriginal = newEnemy.transform.localScale;
            Quaternion rotacionOriginal = newEnemy.transform.rotation;
            newEnemy.transform.SetParent(this.transform, true);
            yield return null;
            newEnemy.transform.localScale = escalaOriginal;
            newEnemy.transform.rotation = rotacionOriginal;
            newEnemy.level = this;
        }

        // Actualizamos el tiempo transcurrido
        tiempoDesdeUltimoSpawn = delayGrupo;
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