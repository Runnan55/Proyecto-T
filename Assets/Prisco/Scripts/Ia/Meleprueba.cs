using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class Meleprueba : EnemyLife
{
    [Header("Audio")]
    [SerializeField] private FMODUnity.EventReference enemySwoosh;

    [Header("Attack Settings")]
    public Transform AttackSpawn; 
    public GameObject attackEffectPrefab;
    public float attackDistance = 2f;
    public float movimientoAtaque = 5f;
    public float attackMoveSpeed = 5f; 
    public float moveTime = 2f; 
    private static List<Meleprueba> allEnemies = new List<Meleprueba>(); // Lista estática de todos los enemigos

  
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
    public float pushForce = 10f;
    public float pushDuration = 0.5f;
    private bool isBeingPushed = false; 
    public float pushCooldown = 0.5f; 
    private float lastPushTime = -1f;

    private Rigidbody rb; 

    private Collider colliderEnemy;

    private float originalAgentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
        colliderEnemy = GetComponent<Collider>(); 

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRenderer = GetComponent<Renderer>(); 
        currentState = State.Chasing;
        previousState = currentState; 
        originalAgentSpeed = agent.speed; // Almacena la velocidad para editarla mejor
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

    void Start()
    {
     
    }

    void Update()
    {
       
        if (currentState != previousState)
        {
            Debug.Log("Entering " + currentState + " state");
            previousState = currentState;
        }

        switch (currentState)
        {         
            case State.Chasing:
                HandleChase();
                break;
            case State.Attacking:
                HandleAttack();
                break; 
                case State.Gethit:
                HandleStateHited();
                break; 
                             
        }

        agent.speed = originalAgentSpeed * MovimientoJugador.bulletTimeScale;
    }

    private void HandleStateHited()
    {
       //por si hay que poner algo en este estado
    }
#region Recivir Daño
    public override void ReceiveDamage(float damage)
    {
        base.ReceiveDamage(damage);   
        colliderEnemy.isTrigger = false;    

          
        if (Time.time > lastPushTime + pushCooldown)
        {
            lastPushTime = Time.time;
            StartCoroutine(PushBack());
          
        }
          currentState = State.Gethit;

    }
    private IEnumerator PushBack()
    {
        
        if (rb == null) yield break; 

        isBeingPushed = true;
        agent.enabled = false; 
        rb.isKinematic = false;
       enemyRenderer.material.color = Color.red; 

        // Aplica una fuerza de empuje en la dirección opuesta a la posición del jugador
        Vector3 pushDirection = (transform.position - player.position).normalized;

        if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
        else if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            pushForce = 15f;
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
        else if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            pushForce = 20f;
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

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Restaurar el estado del Rigidbody y NavMeshAgent
        rb.isKinematic = true; 
        agent.enabled = true; 
        isBeingPushed = false;
        currentState = State.Chasing;
    }  

 #endregion

#region Perseguir y Atacar
    private void HandleChase()
    {       
        if (!agent.enabled)
        {
            agent.enabled = true; 
        }

        agent.isStopped = false; 

        // Calcular una posición alrededor del jugador
        Vector3 offset = Vector3.zero;
        foreach (Meleprueba enemy in allEnemies)
        {
            if (enemy != this && enemy.currentState == State.Attacking)
            {
                offset += (transform.position - enemy.transform.position).normalized;
            }
        }

        Vector3 targetPosition = player.position + offset.normalized * attackDistance;
        agent.SetDestination(targetPosition);

        enemyRenderer.material.color = Color.white; 

        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            currentState = State.Attacking;
        }
       
        
    }
#endregion

#region Ejecutar Ataque
    private void HandleAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        // Anticipacion Ataque
        isAttacking = true;
        agent.isStopped = true; 
        float elapsedTime = 0f;
        Color initialColor = enemyRenderer.material.color;
        Color targetColor = Color.blue;

        while (elapsedTime  < 0.5f)
        {
            if (currentState != State.Attacking)
            {
                isAttacking = false;
                yield break; // Salir de la corrutina si el estado ha cambiado
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
        
        // Activar el efecto una vez
        FMODUnity.RuntimeManager.PlayOneShot(enemySwoosh);

        GameObject effect = Instantiate(attackEffectPrefab, AttackSpawn.position, Quaternion.identity);        
        effect.transform.parent = transform; 
        
        while (elapsedTime  < 1f )
        {
            colliderEnemy.isTrigger = true;
            if (currentState != State.Attacking)
            {
                colliderEnemy.isTrigger = false;
                Destroy(effect); // Destruye el efecto si el estado ha cambiado
                isAttacking = false;
                yield break; // Salir de la corrutina si el estado ha cambiado
            }

            elapsedTime += Time.deltaTime * MovimientoJugador.bulletTimeScale;
            yield return null;
        }

       effect.SetActive(false); // Desactiva el efecto

        // Recuperacion
        Vector3 RestartdirectionToPlayer = (player.position - transform.position).normalized;
        RestartdirectionToPlayer.y = 0; 
        Quaternion RestartlookRotation = Quaternion.LookRotation(RestartdirectionToPlayer);   
        transform.rotation = Quaternion.Slerp(transform.rotation, RestartlookRotation, Time.deltaTime * 15);     

        enemyRenderer.material.color = Color.white; 

        agent.isStopped = false;
        isAttacking = false;
        colliderEnemy.isTrigger = false;

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
                yield break; // Salir de la corrutina si el estado ha cambiado
            }

            Vector3 newPosition = Vector3.Lerp(start, destination, elapsed / duration);
            newPosition.y = start.y;

            // Check for walls using raycasting
            Vector3 direction = newPosition - transform.position;
            float distance = direction.magnitude;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, LayerMask.GetMask("obstacleLayers")))
            {
                // If a wall is hit, stop the movement
                transform.position = hit.point;
                yield break;
            }

            transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        destination.y = start.y;
        transform.position = destination;
    }
#endregion

}
