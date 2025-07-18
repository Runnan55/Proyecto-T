using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemigosMele : EnemyLife
{
    [Header("Audio")]
    [SerializeField] private FMODUnity.EventReference enemySwoosh;

    [Header("Attack Settings")]
    public Transform AttackSpawn; 
    public GameObject attackEffectPrefab;
    public float attackDistance = 2f;
    public float movimientoAtaque = 5f;
    public float attackMoveSpeed = 5f; // Velocidad de movimiento durante el ataque  
    public float moveTime = 2f; // Tiempo en segundos
    private static List<EnemigosMele> allEnemies = new List<EnemigosMele>(); // Lista estática de todos los enemigos
    private static List<EnemigosMele> currentAttackers = new List<EnemigosMele>(); // Lista de enemigos que están atacando actualmente
    private const int maxAttackers = 2; // Máximo número de enemigos que pueden atacar al mismo tiempo

  
    public enum State { Chasing, Attacking, Gethit }
   
    public State currentState;
    private State previousState; 

    [Header("Components")]
    private NavMeshAgent agent;
    private Transform player;    
    private Renderer enemyRenderer; 

    [Header("Attack State")]
    private Vector3 lastPlayerPosition;
    private bool isAttacking = false;

     [Header("Empuje al Recibir Daño")]
    public float pushForce = 10f; // Fuerza del empuje
    public float pushDuration = 0.5f; // Duración del empuje
    private bool isBeingPushed = false; // Estado para controlar si está en empuje
    public float pushCooldown = 0.5f; // Cooldown para evitar empujes continuos
    private float lastPushTime = -1f; // Último tiempo de empuje

    private Rigidbody rb; // Rigidbody del enemigo

    private float originalAgentSpeed;
    private bool hasBeenAdded = false; // Para evitar duplicados en la lista


     private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Intenta obtener el Rigidbody

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRenderer = GetComponent<Renderer>(); 
        currentState = State.Chasing;
        previousState = currentState; 
        originalAgentSpeed = agent.speed; // Almacena la velocidad original del agente

         if (!allEnemies.Contains(this))
        {
            allEnemies.Add(this);
            hasBeenAdded = true;
        }
    }

    void OnDisable()
    {
        // Remover este enemigo de las listas cuando se desactive
        if (allEnemies.Contains(this))
        {
            allEnemies.Remove(this);
        }
        if (currentAttackers.Contains(this))
        {
            currentAttackers.Remove(this);
        }
    }

    void OnDestroy()
    {
        // Asegurar limpieza al destruir el objeto
        if (allEnemies.Contains(this))
        {
            allEnemies.Remove(this);
        }
        if (currentAttackers.Contains(this))
        {
            currentAttackers.Remove(this);
        }
    }

     void Update()
    {       
        // Verificar que las listas no tengan referencias nulas
        CleanupLists();
        
        switch (currentState)
        {         
            case State.Chasing:
                Chase();
                break;
                
            case State.Attacking:
                Attack();
                break; 
                
            case State.Gethit:
                // No hacer nada en gethit, el estado cambiará automáticamente
                break; 
        }

        agent.speed = originalAgentSpeed * MovimientoJugador.bulletTimeScale;
    }

    private void CleanupLists()
    {
        // Limpiar referencias nulas de las listas estáticas
        allEnemies.RemoveAll(enemy => enemy == null);
        currentAttackers.RemoveAll(enemy => enemy == null);
    }

    public override void ReceiveDamage(float damage)
    {
        base.ReceiveDamage(damage); // Llama al método base para reducir la vida y gestionar el cambio de material       

          // Verifica si puede aplicar el empuje
        if (Time.time > lastPushTime + pushCooldown)
        {
            lastPushTime = Time.time;
            StartCoroutine(PushBack());
          
        }
          currentState = State.Gethit;

    }
    private IEnumerator PushBack()
    {
        if (rb == null) yield break; // Salir si no hay Rigidbody

        isBeingPushed = true;
        
        // Desactivar agente de forma segura
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
            yield return new WaitForEndOfFrame();
            agent.enabled = false;
        }
        
        rb.isKinematic = false;
        enemyRenderer.material.color = Color.red; 

        // Aplica una fuerza de empuje en la dirección opuesta a la posición del jugador
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
                rb.velocity = Vector3.zero; // Detener el empuje
                break;
            }

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.isKinematic = true;
        
        // Reactivar agente de forma segura
        yield return new WaitForEndOfFrame();
        
        if (agent != null)
        {
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
        }
        
        isBeingPushed = false;
        currentState = State.Chasing;
    }  

 

    private void Chase()
    {       
        if (agent != null && !agent.enabled && !isBeingPushed)
        {
            agent.enabled = true;
        }

        if (agent != null && agent.enabled)
        {
            try
            {
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    
                    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                    
                    // Verificar si puede atacar (dentro del rango Y hay espacio disponible O ya está en la lista de atacantes)
                    bool canAttack = distanceToPlayer <= attackDistance && 
                                    (currentAttackers.Count < maxAttackers || currentAttackers.Contains(this));

                    if (canAttack)
                    {
                        // Cambiar a estado de ataque
                        currentState = State.Attacking;
                        if (!currentAttackers.Contains(this))
                        {
                            currentAttackers.Add(this);
                            Debug.Log($"Enemigo {gameObject.name} agregado a atacantes. Total atacantes: {currentAttackers.Count}");
                        }
                        agent.SetDestination(player.position);
                        enemyRenderer.material.color = Color.red; // Color de ataque
                    }
                    else if (currentAttackers.Count >= maxAttackers && !currentAttackers.Contains(this))
                    {
                        // Si hay demasiados atacantes, mantener distancia
                        Vector3 directionToPlayer = (player.position - transform.position).normalized;
                        Vector3 targetPosition = player.position - directionToPlayer * (attackDistance + 1f);
                        
                        agent.SetDestination(targetPosition);
                        enemyRenderer.material.color = Color.yellow; // Color para indicar que está esperando
                    }
                    else
                    {
                        // Perseguir normalmente
                        agent.SetDestination(player.position);
                        enemyRenderer.material.color = Color.green; // Color de persecución
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error en Chase() con NavMeshAgent: " + ex.Message);
                StartCoroutine(ReactivateAgent());
            }
        }
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            Debug.Log($"Enemigo {gameObject.name} iniciando ataque");
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        
        // Verificar que el agente esté activo antes de usarlo
        try
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Error al detener agente en PerformAttack: " + ex.Message);
        }
        
        float elapsedTime = 0f;
        Color initialColor = enemyRenderer.material.color;
        Color targetColor = initialColor * 0.2f; 

        // Preparación del ataque
        while (elapsedTime < 0.5f)
        {
            if (currentState != State.Attacking)
            {
                isAttacking = false;
                if (currentAttackers.Contains(this))
                {
                    currentAttackers.Remove(this);
                }
                yield break; 
            }

            lastPlayerPosition = player.position;
            Vector3 directionToPlayer = (lastPlayerPosition - transform.position).normalized;
            directionToPlayer.y = 0; 

            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20f); 

            enemyRenderer.material.color = Color.Lerp(initialColor, targetColor, elapsedTime / 0.5f); 
            
            elapsedTime += Time.deltaTime * MovimientoJugador.bulletTimeScale;	
            yield return null;
        }

        // Ejecutar ataque
        try
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Error al reanudar agente en PerformAttack: " + ex.Message);
        }
        
        enemyRenderer.material.color = targetColor; 

        Vector3 finalDirectionToPlayer = (lastPlayerPosition - transform.position).normalized;
        finalDirectionToPlayer.y = 0;

        Vector3 targetPosition = lastPlayerPosition + finalDirectionToPlayer * movimientoAtaque;
        targetPosition.y = transform.position.y; 

        StartCoroutine(MovimientoAtaque(targetPosition, moveTime / MovimientoJugador.bulletTimeScale)); 

        try
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Error al detener agente después del movimiento: " + ex.Message);
        }

        elapsedTime = 0f;        
        
        FMODUnity.RuntimeManager.PlayOneShot(enemySwoosh);

        GameObject effect = Instantiate(attackEffectPrefab, AttackSpawn.position, Quaternion.identity); // <-- AQUÍ se crea el objeto que contiene el daño
        effect.transform.parent = transform; 
        
        // Duración del ataque
        while (elapsedTime < 1f)
        {
            if (currentState != State.Attacking)
            {
                if (effect != null) Destroy(effect); 
                isAttacking = false;
                if (currentAttackers.Contains(this))
                {
                    currentAttackers.Remove(this);
                }
                yield break; 
            }

            elapsedTime += Time.deltaTime * MovimientoJugador.bulletTimeScale;
            yield return null;
        }

        if (effect != null) effect.SetActive(false); 

        // Recuperación
        Vector3 RestartdirectionToPlayer = (player.position - transform.position).normalized;
        RestartdirectionToPlayer.y = 0; 
        Quaternion RestartlookRotation = Quaternion.LookRotation(RestartdirectionToPlayer);   
        transform.rotation = Quaternion.Slerp(transform.rotation, RestartlookRotation, Time.deltaTime * 15);     

        enemyRenderer.material.color = initialColor; 

        try
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Error al reanudar agente al finalizar ataque: " + ex.Message);
            // Intentar reactivar el agente
            StartCoroutine(ReactivateAgent());
        }
        
        isAttacking = false;

        // Liberar al atacante y volver a perseguir
        if (currentAttackers.Contains(this))
        {
            currentAttackers.Remove(this);
            Debug.Log($"Enemigo {gameObject.name} terminó ataque. Atacantes restantes: {currentAttackers.Count}");
        }
        
        currentState = State.Chasing; 
    }

    private IEnumerator ReactivateAgent()
    {
        yield return new WaitForEndOfFrame();
        
        if (agent != null)
        {
            agent.enabled = false;
            yield return new WaitForEndOfFrame();
            
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(transform.position, out navHit, 10f, NavMesh.AllAreas))
            {
                transform.position = navHit.position;
                agent.enabled = true;
                
                if (agent.isOnNavMesh)
                {
                    agent.Warp(transform.position);
                }
            }
        }
    }

    IEnumerator MovimientoAtaque(Vector3 destination, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (currentState != State.Attacking)
            {
                yield break; 
            }

            Vector3 newPosition = Vector3.Lerp(start, destination, elapsed / duration);
            newPosition.y = start.y; 
            transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        destination.y = start.y; 
        transform.position = destination; 
    }

}
