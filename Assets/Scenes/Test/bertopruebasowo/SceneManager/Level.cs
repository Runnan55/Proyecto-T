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
    float tiempoDesdeUltimoSpawn = 0f; // Esta variable mantendrá el tiempo transcurrido desde el último spawn

    foreach (EnemySpawner enemySpawner in wave.enemySpawns)
    {
        // Esperar el tiempo de delay desde el último enemigo
        yield return new WaitForSeconds(enemySpawner.spawnDelay - tiempoDesdeUltimoSpawn);

        // Instanciar la advertencia en el punto de spawn
        GameObject warningInstance = Instantiate(warning, enemySpawner.spawnPoint.position, Quaternion.identity);

        // Hacer que la advertencia mire hacia la cámara
        warningInstance.transform.LookAt(Camera.main.transform);

        // Esperar 1 segundo para que la advertencia sea visible, luego destruirla
        yield return new WaitForSeconds(1f);
        Destroy(warningInstance);

        // Instanciar el enemigo en el punto de spawn
        Enemy newEnemy = Instantiate(enemySpawner.enemy, enemySpawner.spawnPoint.position, Quaternion.identity);

        // Guardar y restablecer la escala y la rotación originales
        Vector3 escalaOriginal = newEnemy.transform.localScale;
        Quaternion rotacionOriginal = newEnemy.transform.rotation;

        // Establecer el padre del enemigo
        newEnemy.transform.SetParent(this.transform, true);

        // Esperar un frame antes de restaurar la escala y rotación
        yield return null;

        newEnemy.transform.localScale = escalaOriginal;
        newEnemy.transform.rotation = rotacionOriginal;

        // Asignar el nivel actual al enemigo
        newEnemy.level = this;

        // Actualizar el tiempo transcurrido desde el último spawn
        tiempoDesdeUltimoSpawn = enemySpawner.spawnDelay;
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