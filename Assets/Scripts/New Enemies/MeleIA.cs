using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleIA : EnemyLife
{
    public enum EnemyState { Searching, Chasing, PreparingToAttack, Attacking }
    public EnemyState currentState;

    [Header("Configuración de Movimiento")]
    public float chaseDistance = 10f; // Distancia a la que comienza a perseguir
    public float attackDistance = 2f;  // Distancia a la que puede atacar
    public float attackCooldown = 2f;  // Tiempo entre ataques

    private NavMeshAgent agent;        // Referencia al agente de NavMesh
    private float attackTimer;         // Temporizador para controlar los ataques
    private Transform player;          // Referencia al transform del jugador

    [Header("Empuje al Recibir Daño")]
    public float pushForce = 10f; // Fuerza del empuje
    public float pushDuration = 0.5f; // Duración del empuje
    private bool isBeingPushed = false; // Estado para controlar si está en empuje
    public float pushCooldown = 0.5f; // Cooldown para evitar empujes continuos
    private float lastPushTime = -1f; // Último tiempo de empuje

    private Rigidbody rb; // Referencia al Rigidbody del enemigo

    [Header("Cubo de Estado")]
    public GameObject statusCube; // Referencia al cubo que cambiará de color

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = EnemyState.Searching;
        attackTimer = 0; // Comienza el cooldown en cero

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'");
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("El enemigo necesita un Rigidbody para el empuje.");
        }

        UpdateStatusCubeColor(); // Cambia el color del cubo al iniciar
    }

    void Update()
    {
        if (player == null || isBeingPushed) return; // Detiene el comportamiento si está siendo empujado

        LookAtPlayer();

        switch (currentState)
        {
            case EnemyState.Searching:
                SearchForPlayer();
                break;
            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.PreparingToAttack:
                PrepareToAttack();
                break;
            case EnemyState.Attacking:
                AttackPlayer();
                break;
        }

        // Actualiza el color del cubo en cada frame
        UpdateStatusCubeColor();

        // Gestionar el temporizador de cooldown
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime; // Resta el tiempo transcurrido
        }
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
    }

    private IEnumerator PushBack()
    {
        if (rb == null) yield break; // Salir si no hay Rigidbody

        isBeingPushed = true;
        agent.enabled = false; // Desactiva el NavMeshAgent
        rb.isKinematic = false; // Cambia el Rigidbody a modo no-kinemático para aplicar la física

        // Aplica una fuerza de empuje en la dirección opuesta a la posición del jugador
        Vector3 pushDirection = (transform.position - player.position).normalized;
        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);

        yield return new WaitForSeconds(pushDuration);

        // Restaurar el estado del Rigidbody y NavMeshAgent
        rb.isKinematic = true; // Cambia de nuevo a kinemático
        agent.enabled = true; // Reactiva el NavMeshAgent
        isBeingPushed = false;
    }

    // Método para localizar al jugador
    private void SearchForPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= chaseDistance)
        {
            currentState = EnemyState.Chasing;
        }
    }

    // Método para perseguir al jugador
    private void ChasePlayer()
    {
        if (!agent.enabled) return; // Salir si el NavMeshAgent está desactivado

        agent.SetDestination(player.position); // Establece la posición del jugador como destino

        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            currentState = EnemyState.PreparingToAttack; // Cambia a estado de preparación para atacar
            agent.isStopped = true; // Detiene al agente para el ataque
        }
        else if (Vector3.Distance(transform.position, player.position) > chaseDistance)
        {
            currentState = EnemyState.Searching; // Vuelve a buscar si el jugador se aleja demasiado
        }
    }

    // Método para preparar el ataque
    private void PrepareToAttack()
    {
        if (attackTimer <= 0)
        {
            currentState = EnemyState.Attacking; // Cambia al estado de ataque
        }
    }

    // Método para atacar al jugador
    private void AttackPlayer()
    {
        // Hacer que el enemigo mire al jugador
        Vector3 direction = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Rotación suave hacia el jugador

        // Si el jugador está en el rango de ataque y el cooldown ha terminado
        if (attackTimer <= 0)
        {
            Debug.Log("El enemigo ha atacado al jugador!");
            player.GetComponent<Life>().ModifyTime(-20); // Infligir daño
            attackTimer = attackCooldown; // Reinicia el temporizador de ataque
        }

        // Si el jugador sale del rango de ataque, vuelve a perseguir
        if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            currentState = EnemyState.Chasing;
            agent.isStopped = false; // Reactiva el movimiento
        }
    }

    private void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void UpdateStatusCubeColor()
    {
        if (statusCube != null)
        {
            Renderer cubeRenderer = statusCube.GetComponent<Renderer>();

            switch (currentState)
            {
                case EnemyState.Searching:
                    cubeRenderer.material.color = Color.green; // Color verde para el estado de búsqueda
                    break;
                case EnemyState.Chasing:
                    cubeRenderer.material.color = Color.yellow; // Color amarillo para el estado de persecución
                    break;
                case EnemyState.PreparingToAttack:
                    cubeRenderer.material.color = Color.magenta; // Color naranja para la preparación del ataque
                    break;
                case EnemyState.Attacking:
                    cubeRenderer.material.color = Color.red; // Color rojo al atacar
                    break;
            }
        }
        else
        {
            Debug.LogError("El cubo de estado no está asignado en el inspector.");
        }
    }
}
