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
        }
    }

    void OnDisable()
    {
        // Remover este enemigo de la lista cuando se desactive
        if (allEnemies.Contains(this))
        {
            allEnemies.Remove(this);
        }
    }

     void Update()
    {       
        
        switch (currentState)
        {         
            case State.Chasing:
                Chase();
                break;
                
            case State.Attacking:
                Attack();
                break; 
                
            case State.Gethit:
                // ...existing code...
                break; 
                             
        }

        agent.speed = originalAgentSpeed * MovimientoJugador.bulletTimeScale;
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
        agent.enabled = false; // Desactiva el NavMeshAgent
        rb.isKinematic = false; // Cambia el Rigidbody a modo no-kinemático para aplicar la física
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

        // Restaurar el estado del Rigidbody y NavMeshAgent
        rb.isKinematic = true; // Cambia de nuevo a kinemático
        agent.enabled = true; // Reactiva el NavMeshAgent
        isBeingPushed = false;
        currentState = State.Chasing;
    }  

 

    private void Chase()
    {       
        if (!agent.enabled)
        {
            agent.enabled = true; // Asegúrate de que el NavMeshAgent esté habilitado
        }

        agent.isStopped = false; // Asegúrate de que el agente no esté detenido

        if (currentAttackers.Count < maxAttackers || currentAttackers.Contains(this))
        {
            agent.SetDestination(player.position);
            enemyRenderer.material.color = Color.green;

            if (Vector3.Distance(transform.position, player.position) < attackDistance)
            {
                currentState = State.Attacking;
                if (!currentAttackers.Contains(this))
                {
                    currentAttackers.Add(this);
                }
            }
        }
        else
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Vector3 targetPosition = player.position + directionToPlayer * attackDistance;

            agent.SetDestination(targetPosition);
            enemyRenderer.material.color = Color.yellow; // Color para indicar que está rodeando
        }
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        
        isAttacking = true;
        agent.isStopped = true; 
        float elapsedTime = 0f;
        Color initialColor = enemyRenderer.material.color;
        Color targetColor = initialColor * 0.2f; 

        while (elapsedTime < 0.5f)
        {
            if (currentState != State.Attacking)
            {
                isAttacking = false;
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

        // Ataque
        agent.isStopped = false;
        enemyRenderer.material.color = targetColor; 

        Vector3 finalDirectionToPlayer = (lastPlayerPosition - transform.position).normalized;
        finalDirectionToPlayer.y = 0;

        Vector3 targetPosition = lastPlayerPosition + finalDirectionToPlayer * movimientoAtaque;
        targetPosition.y = transform.position.y; 

        StartCoroutine(MovimientoAtaque(targetPosition, moveTime / MovimientoJugador.bulletTimeScale)); 

        agent.isStopped = true; 

        elapsedTime = 0f;        
        
        FMODUnity.RuntimeManager.PlayOneShot(enemySwoosh);

        GameObject effect = Instantiate(attackEffectPrefab, AttackSpawn.position, Quaternion.identity);        
        effect.transform.parent = transform; 
        
        while (elapsedTime < 1f)
        {
            if (currentState != State.Attacking)
            {
                Destroy(effect); 
                isAttacking = false;
                currentAttackers.Remove(this); // Liberar al atacante actual
                yield break; 
            }

            elapsedTime += Time.deltaTime * MovimientoJugador.bulletTimeScale;
            yield return null;
        }

        effect.SetActive(false); 

        // Recuperacion
        Vector3 RestartdirectionToPlayer = (player.position - transform.position).normalized;
        RestartdirectionToPlayer.y = 0; 
        Quaternion RestartlookRotation = Quaternion.LookRotation(RestartdirectionToPlayer);   
        transform.rotation = Quaternion.Slerp(transform.rotation, RestartlookRotation, Time.deltaTime * 15);     

        enemyRenderer.material.color = initialColor; 

        agent.isStopped = false;
        isAttacking = false;

        currentAttackers.Remove(this); // Liberar al atacante actual
        currentState = State.Chasing; 
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
