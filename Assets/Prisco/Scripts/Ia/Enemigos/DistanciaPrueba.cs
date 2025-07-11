using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DistanciaPrueba : EnemyLife
{
    public enum State { Chasing, Reposition, Shooting, IsOnRange, GetHit }

    public State currentState;
    private State previousState; 

    [Header("Components")]
    private NavMeshAgent agent;
    private Transform player;    
    private Renderer enemyRenderer; 
    private Rigidbody rb; 
    private float originalAgentSpeed;

    public float distanciaDetecion = 30f;
    public float ReposicionDetecion = 10f;

    public GameObject projectilePrefab;
    public Transform shootingPoint;
    public float waitTimer;
    [SerializeField] private FMODUnity.EventReference crossbow;
    public float waitTimeBetweenShots = 2f;
    public float repositionDelay = 1f; 
    private float repositionTimer; 
    public float preShootDelay = 1f; 
    private bool isPreShooting = false; 

    public float pushForce = 10f; 
    public float pushDuration = 0.5f; 
    private bool isBeingPushed = false; 
    public float pushCooldown = 0.5f; 
    private float lastPushTime = -1f;

     private static List<DistanciaPrueba> allEnemies = new List<DistanciaPrueba>(); 

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

           if (!allEnemies.Contains(this))
        {
            allEnemies.Add(this);
        }
    }

        void OnDisable()
    {
        
        if (allEnemies.Contains(this))
        {
            allEnemies.Remove(this);
        }
    }

    void Update()
    {
        if (currentState != previousState)
        {
            Debug.Log("Entering " + currentState + " state");
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
        case State.Reposition:
            HandleReposition();
            break;
        case State.Shooting:
            HandleShooting();
            break;      
        case State.GetHit:
            HandleGetHit();
            break; 
        }

        LookAtPlayer();
        
        // Verificar que el agente esté activo y en NavMesh antes de modificar velocidad
        if (agent != null && agent.enabled && agent.isOnNavMesh && !isBeingPushed)
        {
            agent.speed = originalAgentSpeed * MovimientoJugador.bulletTimeScale;
            agent.isStopped = (currentState == State.Shooting || currentState == State.IsOnRange);
        }
    }
    #endregion

   

    #region State Handlers
    private void HandleChasing() 
    { 
        // No hacer nada si está siendo empujado
        if (isBeingPushed) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > distanciaDetecion)
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
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
        else
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
            currentState = State.Shooting; 
        }

        // Separarse de otros enemigos y seguir rutas distintas
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector3 separationVector = Vector3.zero;
        Vector3 pathOffset = Vector3.zero;
        int enemyIndex = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].transform == transform)
            {
                enemyIndex = i;
                break;
            }
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].transform != transform)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemies[i].transform.position);
                if (distanceToEnemy < 5f) // Mantener una distancia mínima de 5f entre enemigos
                {
                    Vector3 directionAwayFromEnemy = (transform.position - enemies[i].transform.position).normalized;
                    separationVector += directionAwayFromEnemy * (5f - distanceToEnemy);
                }
            }
        }
    }

    private void HandleReposition() 
    {  
        // No hacer nada si está siendo empujado
        if (isBeingPushed) return;

        repositionTimer -= Time.deltaTime * MovimientoJugador.bulletTimeScale;
        if (repositionTimer <= 0)
        {
            currentState = State.Chasing; 
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 directionToBackOfPlayer = Quaternion.Euler(0, 180, 0) * directionToPlayer;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (enemy.transform != transform)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < 5f) 
                {
                    Vector3 directionAwayFromEnemy = (transform.position - enemy.transform.position).normalized;
                    directionToBackOfPlayer += directionAwayFromEnemy * (5f - distanceToEnemy);
                }

                
                Vector3 enemyToPlayer = (player.position - enemy.transform.position).normalized;
                if (Vector3.Dot(directionToPlayer, enemyToPlayer) > 0.9f) 
                {
                    directionToBackOfPlayer += Quaternion.Euler(0, 90, 0) * directionToPlayer;
                }
            }
        }

        directionToBackOfPlayer.Normalize();
        Vector3 newPosition = player.position + directionToBackOfPlayer * 15f; 
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(newPosition);
        }
    }

    private void HandleShooting() 
    {        
        // No hacer nada si está siendo empujado
        if (isBeingPushed) return;

        LookAtPlayer();
        
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

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

            // Cambiar a estado Reposition después de disparar
            currentState = State.Reposition;
            repositionTimer = repositionDelay; // Iniciar el temporizador de reposicionamiento
        }
    }

    private void HandleGetHit() 
    { 
        // Implementar lógica para el estado GetHit si es necesario
    }
    #endregion

    #region Damage Handling
    public override void ReceiveDamage(float damage)
    {
        base.ReceiveDamage(damage); // Llama al método base para reducir la vida y gestionar el cambio de material       

        // Verifica si puede aplicar el empuje
        if (Time.time > lastPushTime + pushCooldown)
        {
            lastPushTime = Time.time;
            StartCoroutine(PushBack());
        }
        currentState = State.GetHit;
    }

    private IEnumerator PushBack()
    {
        if (rb == null) yield break;

        isBeingPushed = true;
        
        // Desactivar agente de forma segura
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
            yield return new WaitForEndOfFrame(); // Esperar un frame
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
            pushForce = 15;
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

        // Restaurar estado de forma segura
        rb.isKinematic = true;
        
        // Esperar un frame antes de reactivar el agente
        yield return new WaitForEndOfFrame();
        
        if (agent != null)
        {
            // Verificar que esté en una posición válida del NavMesh
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(transform.position, out navHit, 5f, NavMesh.AllAreas))
            {
                transform.position = navHit.position;
                agent.enabled = true;
                
                if (agent.isOnNavMesh)
                {
                    agent.Warp(transform.position);
                    agent.isStopped = false;
                }
            }
            else
            {
                // Si no puede encontrar el NavMesh, intentar teleportar cerca del jugador
                Vector3 fallbackPosition = player.position + Vector3.back * 10f;
                if (NavMesh.SamplePosition(fallbackPosition, out navHit, 10f, NavMesh.AllAreas))
                {
                    transform.position = navHit.position;
                    agent.enabled = true;
                    if (agent.isOnNavMesh)
                    {
                        agent.Warp(transform.position);
                        agent.isStopped = false;
                    }
                }
            }
        }
        
        isBeingPushed = false;
        currentState = State.Chasing;
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
}
