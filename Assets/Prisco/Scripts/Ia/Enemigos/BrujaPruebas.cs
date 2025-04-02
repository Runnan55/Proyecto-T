using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BrujaPruebas : EnemyLife
{
    #region Variables

    [Header("General Variables")]
    public float tiempoRevivir = 3f;
    public Mesh revivirMesh;
    private Mesh originalMesh;
    private MeshFilter meshFilter;
    public GameObject areaEffectPrefab;

    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;   
    private int enemiesSpawned = 0;
    private float spawnTimer = 0f;
    private float spawnInterval = 0.5f; // Intervalo entre cada enemigo generado
    public Transform[] spawnPoints; // Array de puntos de generación
    private int currentSpawnIndex = 0; // Índice del punto de generación actual
    private List<GameObject> spawnedEnemies = new List<GameObject>(); // Lista para rastrear las invocaciones
    public enum State { Chasing, Reposition, Shooting, IsOnRange, GetHit, Spawning }
    public State currentState;
    private State previousState;

    [Header("Movement and Detection")]
    public float distanciaDetecion = 30f;
    public float distanciaDisparo = 15f;
    public float alejarDistancia = 5f;
    public float ReposicionDetecion = 10f;
    private float repositionTimer;
    public float repositionDelay = 1f;
    private Vector3 repositionTarget; // Variable para almacenar el punto de reposicionamiento

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootingPoint;
    public float waitTimer;
    [SerializeField] private FMODUnity.EventReference crossbow;
    public float waitTimeBetweenShots = 2f;
    public float preShootDelay = 1f;
    private bool isPreShooting = false;

    [Header("Push Logic")]
    public float pushForce = 10f;
    public float pushDuration = 0.5f;
    private bool isBeingPushed = false;
    public float pushCooldown = 0.5f;
    private float lastPushTime = -1f;

    [Header("Components")]
    private NavMeshAgent agent;
    private Transform player;
    private Renderer enemyRenderer;
    private Rigidbody rb;
    private float originalAgentSpeed;

    private static List<DistanciaPrueba> allEnemies = new List<DistanciaPrueba>();

    [Header("Reviving")]
    private bool isReviving = false;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRenderer = GetComponent<Renderer>();
        currentState = State.Chasing;
        previousState = currentState;
        originalAgentSpeed = agent.speed;
        meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter != null)
        {
            originalMesh = meshFilter.mesh;
        }
    }

    private void Update()
    {
        if (isReviving) return;

        if (currentState != previousState)
        {
            previousState = currentState;
            if (currentState == State.Shooting)
            {
                waitTimer = preShootDelay;
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > distanciaDetecion && currentState != State.Shooting)
        {
            currentState = State.Chasing;
        }

        switch (currentState)
        {
            case State.Chasing:
                HandleChasing();
                break;
            case State.Shooting:
                HandleShooting();
                break;
            case State.Reposition:
                HandleReposition();
                break;
            case State.Spawning:
                HandleSpawning();
                break;
        }

        LookAtPlayer();
        agent.speed = originalAgentSpeed * MovimientoJugador.bulletTimeScale;
        agent.isStopped = (currentState == State.Shooting || currentState == State.IsOnRange);
    }

    #endregion

    #region State Handlers

    private void HandleChasing()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= distanciaDisparo)
        {
            currentState = State.Shooting;
            return;
        }

        if (agent != null && agent.isOnNavMesh)
        {
            try
            {
                agent.SetDestination(player.position);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error setting destination: " + ex.Message);
            }
        }
    }

    private void HandleReposition()
    {
        if (repositionTimer == repositionDelay) // Generar el punto solo una vez
        {
            Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
            repositionTarget = transform.position + directionAwayFromPlayer * 10f; // Punto en dirección contraria al jugador
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(repositionTarget);
        }

        repositionTimer -= Time.deltaTime * MovimientoJugador.bulletTimeScale;
        if (repositionTimer <= 0)
        {
            currentState = State.Spawning;
            repositionTimer = repositionDelay; // Reiniciar el temporizador
        }
    }

    private void HandleShooting()
    {
        LookAtPlayer();
        agent.isStopped = true;

        if (!isPreShooting)
        {
            isPreShooting = true;
            waitTimer = preShootDelay;
        }

        waitTimer -= Time.deltaTime * MovimientoJugador.bulletTimeScale;
        float lerpFactor = 1 - (waitTimer / preShootDelay);
        enemyRenderer.material.color = Color.Lerp(Color.white, Color.blue, lerpFactor);

        if (waitTimer <= 0)
        {
            enemyRenderer.material.color = Color.white;
            FMODUnity.RuntimeManager.PlayOneShot(crossbow);
            Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

            waitTimer = waitTimeBetweenShots / MovimientoJugador.bulletTimeScale;
            isPreShooting = false;

            currentState = State.Reposition;
        }
    }

    private void HandleSpawning()
    {
        if (agent != null)
        {
            agent.isStopped = true; // Detener el agente
            agent.velocity = Vector3.zero; // Asegurarse de que no tenga velocidad
        }

        if (rb != null)
        {
            rb.isKinematic = true; // Evitar movimientos físicos
        }

        // Validar que los puntos de generación estén configurados
        if (spawnPoints == null || spawnPoints.Length < 3)
        {
            Debug.LogError("Los puntos de generación no están configurados correctamente.");
            currentState = State.Chasing; // Cambiar al estado Chasing si hay un problema
            return;
        }

        // Generar enemigos en los puntos específicos
        spawnTimer += Time.deltaTime * MovimientoJugador.bulletTimeScale;
        if (spawnTimer >= spawnInterval && enemiesSpawned < 3)
        {
            Transform spawnPoint = spawnPoints[currentSpawnIndex];
            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedEnemies.Add(spawnedEnemy); // Agregar la invocación a la lista

            currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length; // Ciclar entre los puntos
            enemiesSpawned++;
            spawnTimer = 0f; // Reiniciar el temporizador
        }

        // Reactivar el movimiento del enemigo después de generar todos los enemigos
        if (enemiesSpawned >= 3)
        {
            if (agent != null)
            {
                agent.isStopped = false; // Permitir que el agente se mueva nuevamente
                agent.velocity = Vector3.zero; // Reiniciar la velocidad
            }

            if (rb != null)
            {
                rb.isKinematic = false; // Reactivar movimientos físicos
            }

            enemiesSpawned = 0; // Reiniciar el contador de enemigos generados
            currentSpawnIndex = 0; // Reiniciar el índice de generación
            currentState = State.Chasing; // Volver al estado Chasing después de invocar
        }
    }

    #endregion

    #region Damage and Effects

    public override void CalcularDamage()
    {
        if (Time.time > lastPushTime + pushCooldown)
        {
            lastPushTime = Time.time;
            StartCoroutine(PushBack());
          
        }
          currentState = State.Chasing;
        if (health <= 0)
        {
            RemoveAllSpawnedEnemies(); // Eliminar todas las invocaciones al morir

            if (antiRevivir)
            {
                ApplyAreaEffect();

                if (level != null)
                {
                    level.EnemyDefeated(this);
                }

                Destroy(gameObject, 0.2f);
            }
            else
            {
                if (!antiRevivir)
                {
                    StartCoroutine(DelayedRevivir());
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isReviving) return;  health -= damage;

        // Interrumpir cualquier ejecución actual
        StopAllCoroutines();
        currentState = State.Chasing;

        // Aplicar empuje sin restricciones de cooldown
        StartCoroutine(PushBack());
    }

    private void ApplyAreaEffect()
    {
        if (areaEffectPrefab != null)
        {
            Instantiate(areaEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void RemoveAllSpawnedEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy); // Eliminar cada invocación
            }
        }
        spawnedEnemies.Clear(); // Limpiar la lista
    }

    #endregion

    #region Reviving Mechanic

    private IEnumerator DelayedRevivir()
    {
        yield return null;
        StartCoroutine(Revivir());
    }

    IEnumerator Revivir()
    {
        if (antiRevivir) yield break;

        Debug.Log("reviviendo");
        isReviving = true;

        if (agent != null) agent.enabled = false;
        if (rb != null) rb.isKinematic = true;
        Collider collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        if (revivirMesh != null && meshFilter != null)
        {
            meshFilter.mesh = revivirMesh;
            yield return new WaitForSeconds(tiempoRevivir);
            meshFilter.mesh = originalMesh;
        }
        else
        {
            yield return new WaitForSeconds(tiempoRevivir);
        }

        if (agent != null) agent.enabled = true;
        if (rb != null) rb.isKinematic = false;
        if (collider != null) collider.enabled = true;

        health = 100;
        antiRevivir = false;
        isReviving = false;
    }

    #endregion

    #region Push Logic

    private IEnumerator PushBack()
    {
        if (rb == null) yield break;

        isBeingPushed = true;
        if (agent != null)
        {
            agent.enabled = false;
        }
        rb.isKinematic = false;
        enemyRenderer.material.color = Color.red;

        Vector3 pushDirection = (transform.position - player.position).normalized;

        if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            pushForce = 10f;
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
        else if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            pushForce = 12.5f;
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
        else if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            pushForce = 15f;
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }

        float elapsedTime = 0f;
        while (elapsedTime < pushDuration)
        {
            float frameDistance = rb.velocity.magnitude * Time.fixedDeltaTime;

            if (Physics.Raycast(rb.position, pushDirection, out RaycastHit hit, frameDistance, LayerMask.GetMask("obstacleLayers")))
            {
                rb.velocity = Vector3.zero;
                break;
            }

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.isKinematic = true;
        if (agent != null)
        {
            agent.enabled = true;
            if (agent.isOnNavMesh)
            {
                agent.Warp(transform.position);
            }
        }
        isBeingPushed = false;
    }

    #endregion

    #region Utility Methods

    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20f);
    }

    #endregion

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DisparoCargado>() != null)
        {
            antiRevivir = true;
            if (health <= 0)
            {
                StopAllCoroutines();
                ApplyAreaEffect();

                if (level != null)
                {
                    level.EnemyDefeated(this);
                }

                Destroy(gameObject, 0.2f);
            }
        }
    }
}
