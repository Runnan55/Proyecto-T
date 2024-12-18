using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class EnemySpawner
{
    public EnemyLife enemy;
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

    [Header("Teleporter config")]
    public bool teleportAfterLastWave = false;
    public float teleportDelay = 1f;
    public Teleporter teleporter;

    [Header("Change scene")]
    public bool changeSceneAfterLastWave = false;
    public string sceneToChange = "NombreDeLaEscena";

    [Header("Time config")]
    public bool modifyTime = false;
    public float maxTime = 600f;
    private Life playerLife;
    private bool timerStarted = false; // { added }

    [Header("Warning config")]
    public GameObject warning;
    [SerializeField] private FMODUnity.EventReference waveStart;

    [Header("Waves config")]
    public List<EnemyWave> waves;

    private int currentWave = 0;
    private int defeatedEnemies = 0;
    private bool hasPlayerEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayerEntered)
        {
            if (entranceDoor != null)
            {
                StartCoroutine(entranceDoor.Close());
            }
            //Debug.Log(hasPlayerEntered);
            StartNextWave();
            hasPlayerEntered = true; 
            //Debug.Log(hasPlayerEntered);

            if (modifyTime && !timerStarted) // { modified }
            {
                playerLife = other.GetComponent<Life>();
                if (playerLife != null)
                {
                    playerLife.maxTime = maxTime;
                    playerLife.StartTimer(maxTime);
                    timerStarted = true; // { added }
                }
            }
        }
    }

    public void StartNextWave()
    {
        if (currentWave < waves.Count)
        {
            FMODUnity.RuntimeManager.PlayOneShot(waveStart);
            defeatedEnemies = waves[currentWave].enemySpawns.Count;
            StartCoroutine(SpawnWave(waves[currentWave]));
            currentWave++;
        }
    }

    private IEnumerator SpawnWave(EnemyWave wave)
    {
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
        bool isFirstEnemySpawned = false;

        foreach (var grupoSpawn in spawnsPorTiempo)
        {
            float delayGrupo = grupoSpawn.Key;

            yield return new WaitForSeconds(delayGrupo - tiempoDesdeUltimoSpawn);

            foreach (EnemySpawner enemySpawner in grupoSpawn.Value)
            {
                if (!isFirstEnemySpawned)
                {
                    // Iniciar el temporizador del jugador
                    //Life playerLife = FindObjectOfType<Life>();
                    //if (playerLife != null)
                    //{
                    //    playerLife.StartTimer(playerLife.maxTime);
                    //}
                    isFirstEnemySpawned = true;
                }

                GameObject warningInstance = Instantiate(warning, enemySpawner.spawnPoint.position, Quaternion.identity);
                warningInstance.transform.LookAt(Camera.main.transform);
                yield return new WaitForSeconds(0.25f);
                Destroy(warningInstance);

                EnemyLife newEnemy = Instantiate(enemySpawner.enemy, enemySpawner.spawnPoint.position, Quaternion.identity);
                Vector3 escalaOriginal = newEnemy.transform.localScale;
                Quaternion rotacionOriginal = newEnemy.transform.rotation;
                newEnemy.transform.SetParent(this.transform, true);
                yield return null;
                newEnemy.transform.localScale = escalaOriginal;
                newEnemy.transform.rotation = rotacionOriginal;
                newEnemy.level = this;
            }

            tiempoDesdeUltimoSpawn = delayGrupo;
        }
    }

    public void EnemyDefeated(EnemyLife enemy)
    {
        Debug.Log("Enemy defeated");
        defeatedEnemies--;
        Debug.Log("Defeated enemies: " + defeatedEnemies);

        if (defeatedEnemies <= 0)
        {
            if (currentWave == waves.Count)
            {
                if (entranceDoor != null)
                {
                    StartCoroutine(entranceDoor.Open());
                }

                if (exitDoor != null)
                {
                    StartCoroutine(exitDoor.Open());
                }

                if (teleportAfterLastWave && teleporter != null)
                {
                    StartCoroutine(TeleportPlayerWithDelay());
                }

                if (changeSceneAfterLastWave)
                {
                    SceneManager.LoadScene(sceneToChange);
                }

                if (modifyTime && playerLife != null)
                {
                    playerLife.StopTimer();
                }
            }
            else
            {
                StartNextWave();
            }
        }
    }

    private IEnumerator TeleportPlayerWithDelay()
    {
        yield return new WaitForSeconds(teleportDelay);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && teleporter.exitPoint != null)
        {
            teleporter.Teleport(player, teleporter.exitPoint);
            Debug.Log("Player teleported to exit portal.");
        }
    }
}
//hola