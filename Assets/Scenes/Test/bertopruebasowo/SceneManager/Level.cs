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
    public float teleportDelay = 0.1f;
    public Teleporter teleporter;

    [Header("Change scene")]
    public bool changeSceneAfterLastWave = false;
    public string sceneToChange = "NombreDeLaEscena";

    [Header("Rooms")]
    public bool changeRooms = false;
    public bool isLastRoom = false;
    public GameObject actualRoom;
    public GameObject nextRoom;

    [Header("Time config")]
    public bool modifyTime = false;
    public float maxTime = 600f;
    private Life playerLife;
    private bool timerStarted = false;

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
            StartNextWave();
            hasPlayerEntered = true;

            if (modifyTime && !timerStarted)
            {
                playerLife = other.GetComponent<Life>();
                if (playerLife != null)
                {
                    playerLife.maxTime = maxTime;
                    playerLife.StartTimer(maxTime);
                    timerStarted = true;
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
        // Agrupamos los spawns por su delay
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
                    isFirstEnemySpawned = true;
                }

                // Mostrar advertencia (warning) antes de spawnear al enemigo
                GameObject warningInstance = Instantiate(warning, enemySpawner.spawnPoint.position, Quaternion.identity);
                if (Camera.main != null)
                {
                    warningInstance.transform.LookAt(Camera.main.transform);
                }
                yield return new WaitForSeconds(0.25f);
                Destroy(warningInstance);

                // Instanciar al enemigo
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
        //Debug.Log("Enemy defeated");
        defeatedEnemies--;
        //Debug.Log("Defeated enemies: " + defeatedEnemies);

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

                // Si se activa el teletransporte y/o cambio de sala, se llama a la corrutina unificada
                if (teleportAfterLastWave || changeRooms)
                {
                    StartCoroutine(HandleRoomTransitionAndTeleport());
                }

                if (changeSceneAfterLastWave)
                {
                    // Guardar la siguiente escena en PlayerPrefs
                    PlayerPrefs.SetString("SceneToLoad", sceneToChange);
                    PlayerPrefs.Save();

                    // Pantalla de carga
                    SceneManager.LoadScene("LoadingScene");
                }

                if (modifyTime && playerLife != null)
                {
                    playerLife.AddToTimeBank(playerLife.currentTime);
                    playerLife.ClearTime();
                    playerLife.UpdateTimeBankText();
                    playerLife.StopTimer();
                }
            }
            else
            {
                StartNextWave();
            }
        }
    }

    /// <summary>
    /// Corrutina unificada para activar la siguiente sala (si se requiere), teletransportar al jugador y desactivar la sala actual.
    /// El orden es:
    /// 1. Activar la siguiente sala.
    /// 2. Teletransportar al jugador.
    /// 3. Desactivar la sala actual.
    /// </summary>
    private IEnumerator HandleRoomTransitionAndTeleport()
    {
        // Paso 1: Activar la siguiente sala (si se ha indicado cambiar de sala)
        if (changeRooms && nextRoom != null)
        {
            if (!isLastRoom)
            {
                nextRoom.SetActive(true);
                Debug.Log("Activando la siguiente sala: " + nextRoom.name);
            }
        }

        // Paso 2: Teletransportar al jugador (si se ha indicado)
        if (teleportAfterLastWave && teleporter != null && teleporter.exitPoint != null)
        {
            yield return new WaitForSeconds(teleportDelay);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                teleporter.Teleport(player, teleporter.exitPoint);
                Debug.Log("Jugador teletransportado al exitPoint.");
            }
        }

        // Paso 3: Desactivar la sala actual (si se ha indicado cambiar de sala)
        if (changeRooms && actualRoom != null)
        {
            // Si se realizó teletransporte, esperar un poco antes de desactivar la sala actual
            if (teleportAfterLastWave)
            {
                yield return new WaitForSeconds(1.5f);
            }
            actualRoom.SetActive(false);
            Debug.Log("Desactivando la sala actual: " + actualRoom.name);
        }
    }

    // Las siguientes corrutinas antiguas se pueden eliminar, ya que se han integrado en HandleRoomTransitionAndTeleport.
    /*
    private IEnumerator ChangeRoomsWithDelay()
    {
        nextRoom.SetActive(true);
        Debug.Log("activando nextroom: " + nextRoom);
        yield return new WaitForSeconds(1.5f);
        actualRoom.SetActive(false);
        Debug.Log("desactivando actualroom: " + actualRoom);
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
    */
}
