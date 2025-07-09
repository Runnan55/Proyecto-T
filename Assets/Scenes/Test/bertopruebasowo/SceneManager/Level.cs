using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

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
    public GridManager gridManager;
    public int gridLayoutIndex = 0;
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

    [Header("Camera config")]
    public CinemachineVirtualCamera virtualCamera;
    public float targetFov = 25;
    public float targetRotationX = 40;
    public float targetDistance = 83;
    public bool disableCameraFollowOnStart = false;
    public Vector3 newPosition = new Vector3(0, 10, -10);

    private CinemachineFramingTransposer framingTransposer;

    private void Start()
    {
        if (virtualCamera != null)
        {
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (disableCameraFollowOnStart)
        {
            disableCameraFollow();
        }

        if (!disableCameraFollowOnStart)
        {
            enableCameraFollow();
        }
        
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

            AdjustCamera();
        }
    }

    private void AdjustCamera()
    {
        if (virtualCamera != null)
        {
            virtualCamera.m_Lens.FieldOfView = targetFov;

            Vector3 rotation = virtualCamera.transform.eulerAngles;
            rotation.x = targetRotationX;
            virtualCamera.transform.eulerAngles = rotation;

            if (framingTransposer != null)
            {
                framingTransposer.m_CameraDistance = targetDistance;
            }
        }
    }

    public void StartNextWave()
    {
        if (currentWave < waves.Count)
        {
            FMODUnity.RuntimeManager.PlayOneShot(waveStart);
            defeatedEnemies = waves[currentWave].enemySpawns.Count;

            var wave = waves[currentWave];
            if (wave.gridManager != null)
            {
                wave.gridManager.ActivateWave(wave.gridLayoutIndex);
            }

            StartCoroutine(SpawnWave(wave));
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
                    isFirstEnemySpawned = true;
                }

                GameObject warningInstance = Instantiate(warning, enemySpawner.spawnPoint.position, Quaternion.identity);
                if (Camera.main != null)
                {
                    warningInstance.transform.LookAt(Camera.main.transform);
                }
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
        defeatedEnemies--;

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

                bool seUsoGrid = false;
                foreach (var wave in waves)
                {
                    if (wave.gridManager != null)
                    {
                        seUsoGrid = true;
                        break;
                    }
                }

                if (seUsoGrid)
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        var movimientoJugador = player.GetComponent<MovimientoJugador>();
                        if (movimientoJugador != null)
                        {
                            movimientoJugador.nivelActual = 4;
                            UnityEngine.SceneManagement.SceneManager.LoadScene("HubNuevo");
                        }
                    }
                }

                if (teleportAfterLastWave || changeRooms)
                {
                    StartCoroutine(HandleRoomTransitionAndTeleport());
                }

                if (changeSceneAfterLastWave)
                {
                    PlayerPrefs.SetString("SceneToLoad", sceneToChange);
                    PlayerPrefs.Save();
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

    private IEnumerator HandleRoomTransitionAndTeleport()
    {
        if (changeRooms && nextRoom != null)
        {
            if (!isLastRoom)
            {
                nextRoom.SetActive(true);
                Debug.Log("Activando la siguiente sala: " + nextRoom.name);
            }
        }

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

        if (changeRooms && actualRoom != null)
        {
            if (teleportAfterLastWave)
            {
                yield return new WaitForSeconds(1.5f);
            }
            actualRoom.SetActive(false);
            Debug.Log("Desactivando la sala actual: " + actualRoom.name);
        }
    }

    void enableCameraFollow()
    {
        Debug.Log("Camera follow habilitado.");
        if (virtualCamera != null)
        {
            // Busca el script CameraFollowPlayer en la MainCamera
            CameraFollowPlayer cameraFollowPlayer = Camera.main?.GetComponent<CameraFollowPlayer>();
            if (cameraFollowPlayer != null)
            {
                cameraFollowPlayer.enabled = false;
                Debug.Log("CameraFollowPlayer desactivado para habilitar el seguimiento manual.");
            }

            // Asigna el jugador como objetivo de la cámara
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                virtualCamera.Follow = player.transform;
                Debug.Log("Camera follow habilitado y asignado al jugador.");
            }
            else
            {
                Debug.LogWarning("Player object not found. Camera follow no habilitado.");
            }
        }
        else
        {
            Debug.LogWarning("virtualCamera no está asignada.");
        }
    }

    void disableCameraFollow()
    {
        Debug.Log("Camera follow deshabilitado.");
        if (virtualCamera != null)
        {
            // Desactiva el seguimiento de la cámara
            virtualCamera.Follow = null;

            // Cambia la posición de la cámara a una posición específica
            virtualCamera.transform.position = newPosition;

            // Busca el script CameraFollowPlayer en la MainCamera
            CameraFollowPlayer cameraFollowPlayer = Camera.main?.GetComponent<CameraFollowPlayer>();
            if (cameraFollowPlayer != null)
            {
                cameraFollowPlayer.enabled = false;
                Debug.Log("CameraFollowPlayer desactivado para evitar conflictos.");
            }

            Debug.Log("Camera follow desactivado y posición de la cámara ajustada.");
        }
        else
        {
            Debug.LogWarning("virtualCamera no está asignada.");
        }
    }
}