using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClonesPruebas : EnemyLife
{
    public enum State { Chasing, Attacking, Gethit, Waiting } // Añadir el estado Waiting
    public State currentState;
    private State previousState;
    public static List<ClonesPruebas> allEnemies = new List<ClonesPruebas>();

    [Header("Components")]
    private NavMeshAgent agent;
    private Transform player;    
    private Renderer enemyRenderer; 
    private Rigidbody rb; 

    [Header("Cloning")]
    public GameObject clonePrefab; 
    private GameObject clone1; 
    private GameObject clone2; 
    private List<Vector3> previousPositions = new List<Vector3>(); 
    private Vector3 lastEnemyPosition; 
    private Vector3 lastClonePosition1; 
    private Vector3 lastClonePosition2; 
    private List<Vector3> teleportPositions = new List<Vector3>();

    [Header("Attack")]
    public GameObject bulletPrefab; 
    public Transform bulletSpawnPoint; 
    public float shootDelay = 1f; // Velocidad entre cada disparo
    public float preShootDelay = 1.5f; // Duración del pre-disparo
    public float waitTimer = 1.5f; // Duración del cambio de color
    public float attackCooldown = 5f; // Duración del cooldown de ataque
    private float lastAttackTime; 

    [Header("Movement")]
    public float teleportDistance = 20f; // Distancia de teletransporte
    public float detectionDistance = 20f; // Distancia de detección
    private float originalAgentSpeed; // Velocidad original del agente

    [Header("Push")]
    private bool isBeingPushed = false; 
    private float lastPushTime = -1f; // Tiempo del último empuje
    public float pushCooldown = 2f; // Duración del cooldown de empuje
    public float pushForce = 10f; // Fuerza de empuje
    public float pushDuration = 0.5f; // Duración del empuje

    [Header("State Timers")]
    public float waitDuration = 5f; // Duración del estado de espera

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();       
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRenderer = GetComponent<Renderer>();
        currentState = State.Chasing;
        previousState = currentState; 
        lastAttackTime = -attackCooldown; 
        originalAgentSpeed = agent.speed; 
       
        if (!allEnemies.Contains(this))
        {
            allEnemies.Add(this);
        }

        
        teleportPositions.Add(Vector3.zero); 
        teleportPositions.Add(Vector3.zero); 
        teleportPositions.Add(Vector3.zero); 
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Chasing:
                Chasing();
                break;
            case State.Attacking:
                Attacking();
                break;
            case State.Gethit:
                GetHit();
                break;
            case State.Waiting:
                Waiting();
                break;
        }
        agent.speed = originalAgentSpeed * MovimientoJugador.bulletTimeScale; 

       
        if (MovimientoJugador.bulletTimeScale < 1)
        {
            if (clone1 != null)
            {
                clone1.GetComponent<Renderer>().material.color = Color.red;
            }
            if (clone2 != null)
            {
                clone2.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        else
        {
            if (clone1 != null)
            {
                clone1.GetComponent<Renderer>().material.color = CloneBehavior.cloneRenderer.material.color;
            }
            if (clone2 != null)
            {
                clone2.GetComponent<Renderer>().material.color = CloneBehavior.cloneRenderer.material.color;
            }
        }
    }

    private void Chasing()
    {
        agent.SetDestination(player.position);
        if (Vector3.Distance(transform.position, player.position) <= detectionDistance)
        {
            currentState = State.Attacking;
        }
    }    

    private void Attacking()
    {
        if (Time.time - lastAttackTime < attackCooldown * MovimientoJugador.bulletTimeScale)
        {
            return; 
        }

        lastAttackTime = Time.time; 

        // Calcular las posiciones de los clones y del enemigo en forma de triángulo
        Vector3 cloneDirection1 = (Quaternion.Euler(0, 0, 0) * Vector3.forward).normalized;
        Vector3 cloneDirection2 = (Quaternion.Euler(0, 120, 0) * Vector3.forward).normalized;
        Vector3 cloneDirection3 = (Quaternion.Euler(0, -120, 0) * Vector3.forward).normalized;
        teleportPositions[0] = player.position + cloneDirection1 * teleportDistance;
        teleportPositions[1] = player.position + cloneDirection2 * teleportDistance;
        teleportPositions[2] = player.position + cloneDirection3 * teleportDistance;

       
        NavMeshHit hit;
        for (int i = 0; i < teleportPositions.Count; i++)
        {
            if (NavMesh.SamplePosition(teleportPositions[i], out hit, 1.0f, NavMesh.AllAreas))
            {
                teleportPositions[i] = hit.position;
                Vector3 tempPosition = teleportPositions[i];
                tempPosition.y = transform.position.y; // Mantener la misma altura
                teleportPositions[i] = tempPosition;
            }
        }

        // Eliminar posiciones duplicadas
        List<Vector3> uniquePositions = new List<Vector3>(teleportPositions);
        for (int i = 0; i < uniquePositions.Count; i++)
        {
            for (int j = i + 1; j < uniquePositions.Count; j++)
            {
                if (uniquePositions[i] == uniquePositions[j])
                {
                    uniquePositions[j] = GetUniquePosition(player.position, teleportDistance, uniquePositions);
                }
            }
        }
        teleportPositions = uniquePositions;

        // Seleccionar las posiciones de los clones y del enemigo
        List<Vector3> availablePositions = new List<Vector3>(teleportPositions);
        Vector3 enemyPosition = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];
        availablePositions.Remove(enemyPosition);
        Vector3 clonePosition1 = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];
        availablePositions.Remove(clonePosition1);
        Vector3 clonePosition2 = availablePositions[0];

   
        previousPositions.Clear();
        previousPositions.Add(enemyPosition);
        previousPositions.Add(clonePosition1);
        previousPositions.Add(clonePosition2);

        
        lastEnemyPosition = enemyPosition;
        lastClonePosition1 = clonePosition1;
        lastClonePosition2 = clonePosition2;

     
        agent.Warp(enemyPosition);
        float distance = Vector3.Distance(enemyPosition, player.position);
        Debug.Log("Enemy teleported to a distance of: " + distance + " units from the player.");

       
        if (clone1 == null)
        {
            clone1 = Instantiate(clonePrefab, clonePosition1, Quaternion.identity);
        }
        else
        {
            clone1.transform.position = clonePosition1;
        }

        if (clone2 == null)
        {
            clone2 = Instantiate(clonePrefab, clonePosition2, Quaternion.identity);
        }
        else
        {
            clone2.transform.position = clonePosition2;
        }

       
        clone1.transform.position = new Vector3(clone1.transform.position.x, transform.position.y, clone1.transform.position.z);
        clone2.transform.position = new Vector3(clone2.transform.position.x, transform.position.y, clone2.transform.position.z);

        // Asignar las propiedades de los clones
        CloneBehavior cloneBehavior1 = clone1.GetComponent<CloneBehavior>();
        cloneBehavior1.bulletPrefab = bulletPrefab;
        cloneBehavior1.bulletSpawnPoint = clone1.transform.Find("BulletSpawnPoint"); 
        cloneBehavior1.shootDelay = shootDelay;
        cloneBehavior1.preShootDelay = preShootDelay;
        cloneBehavior1.waitTimer = waitTimer;
        cloneBehavior1.lastPosition = clonePosition1;

        CloneBehavior cloneBehavior2 = clone2.GetComponent<CloneBehavior>();
        cloneBehavior2.bulletPrefab = bulletPrefab;
        cloneBehavior2.bulletSpawnPoint = clone2.transform.Find("BulletSpawnPoint");
        cloneBehavior2.shootDelay = shootDelay;
        cloneBehavior2.preShootDelay = preShootDelay;
        cloneBehavior2.waitTimer = waitTimer;
        cloneBehavior2.lastPosition = clonePosition2; 

       
        StartCoroutine(ChangeColorAndShoot());

       
    }

    private Vector3 GetUniquePosition(Vector3 center, float distance, List<Vector3> existingPositions)
    {
        Vector3 newPosition;
        NavMeshHit hit;
        do
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere.normalized;
            newPosition = center + randomDirection * distance;
        } while (!NavMesh.SamplePosition(newPosition, out hit, 1.0f, NavMesh.AllAreas) || existingPositions.Contains(newPosition));
        newPosition = hit.position;
        newPosition.y = transform.position.y; 
        return newPosition;
    }

    private IEnumerator ChangeColorAndShoot()
    {
        bool isPreShooting = true;
        Color originalColor = enemyRenderer.material.color; 
        Color secondaryColor = new Color(1.0f, 0.5f, 0.0f); 
        float elapsedTime = 0f; 

        while (isPreShooting)
        {
            elapsedTime += Time.deltaTime * MovimientoJugador.bulletTimeScale; 
            float lerpFactor = Mathf.Clamp01(elapsedTime / preShootDelay); 
            enemyRenderer.material.color = Color.Lerp(originalColor, secondaryColor, lerpFactor); 

            
            if (clone1 != null)
            {
                clone1.GetComponent<Renderer>().material.color = Color.Lerp(originalColor, secondaryColor, lerpFactor);
            }
            if (clone2 != null)
            {
                clone2.GetComponent<Renderer>().material.color = Color.Lerp(originalColor, secondaryColor, lerpFactor);
            }

            if (elapsedTime >= preShootDelay)
            {
                enemyRenderer.material.color = originalColor; 
                if (clone1 != null)
                {
                    clone1.GetComponent<Renderer>().material.color = CloneBehavior.cloneRenderer.material.color;
                }
                if (clone2 != null)
                {
                    clone2.GetComponent<Renderer>().material.color = CloneBehavior.cloneRenderer.material.color;
                }
                isPreShooting = false; 
                
                
                for (int i = 0; i < 3; i++)
                {
                    Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                    yield return new WaitForSeconds(shootDelay * MovimientoJugador.bulletTimeScale); 
                }

                
                currentState = State.Waiting;
            }
            yield return null;
        }
    }

    private void GetHit()
    {
        
        if (clone1 != null)
        {
            Destroy(clone1);
        }
        if (clone2 != null)
        {
            Destroy(clone2);
        }

        
        currentState = State.Waiting;
    }

    private void Waiting()
    {
        waitDuration -= Time.deltaTime * MovimientoJugador.bulletTimeScale; 
        if (waitDuration <= 0)
        {
           
            if (clone1 != null)
            {
                Destroy(clone1);
            }
            if (clone2 != null)
            {
                Destroy(clone2);
            }

            currentState = State.Chasing;
            waitDuration = 5f; 
        }
    }

    #region Damage Handling
    public override void ReceiveDamage(float damage)
    {
        base.ReceiveDamage(damage);        

        
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
        currentState = State.Chasing;
    }
    #endregion
}
