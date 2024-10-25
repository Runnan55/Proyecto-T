using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleIA : EnemyLife
{
    public enum EnemyState { Searching, Chasing, Attacking }
    public EnemyState currentState;

    [Header("Configuraci�n de Movimiento")]
    public float chaseDistance = 10f; // Distancia a la que comienza a perseguir
    public float attackDistance = 2f;  // Distancia a la que puede atacar
    public float attackCooldown = 2f;  // Tiempo entre ataques

    private NavMeshAgent agent;        // Referencia al agente de NavMesh
    private float attackTimer;         // Temporizador para controlar los ataques
    private Transform player;          // Referencia al transform del jugador

    [Header("Cubo de Estado")]
    public GameObject statusCube;      // Referencia al cubo que cambiar� de color

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = EnemyState.Searching;
        attackTimer = 0; // Comienza el cooldown en cero

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontr� un objeto con la etiqueta 'Player'");
        }

        UpdateStatusCubeColor(); // Cambia el color del cubo al iniciar
    }

    void Update()
    {
        if (player == null) return; // Si no se encontr� el jugador, no se ejecuta el resto

        switch (currentState)
        {
            case EnemyState.Searching:
                SearchForPlayer();
                break;
            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.Attacking:
                AttackPlayer();
                break;
        }

        UpdateStatusCubeColor(); // Actualiza el color del cubo en cada frame

        // Gestionar el temporizador de cooldown
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime; // Resta el tiempo transcurrido
        }
    }

    // M�todo para localizar al jugador
    private void SearchForPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= chaseDistance)
        {
            currentState = EnemyState.Chasing;
        }
    }

    // M�todo para perseguir al jugador
    private void ChasePlayer()
    {
        agent.SetDestination(player.position); // Establece la posici�n del jugador como destino

        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            currentState = EnemyState.Attacking;
            agent.isStopped = true; // Detiene al agente para el ataque
        }
        else if (Vector3.Distance(transform.position, player.position) > chaseDistance)
        {
            currentState = EnemyState.Searching; // Vuelve a buscar si el jugador se aleja demasiado
        }
    }

    // M�todo para atacar al jugador
    private void AttackPlayer()
    {
        // Hacer que el enemigo mire al jugador
        Vector3 direction = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Rotaci�n suave hacia el jugador

        // Si el jugador est� en el rango de ataque y el cooldown ha terminado
        if (attackTimer <= 0)
        {
            Debug.Log("El enemigo ha atacado al jugador!");
            player.GetComponent<Life>().ModifyTime(-20); // Infligir da�o
            attackTimer = attackCooldown; // Reinicia el temporizador de ataque
        }

        // Si el jugador sale del rango de ataque, vuelve a perseguir
        if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            currentState = EnemyState.Chasing;
            agent.isStopped = false; // Reactiva el movimiento
        }
    }

    /// <summary>
    /// Cambia el color del cubo seg�n el estado del enemigo.
    /// </summary>
    private void UpdateStatusCubeColor()
    {
        if (statusCube != null)
        {
            Renderer cubeRenderer = statusCube.GetComponent<Renderer>();

            switch (currentState)
            {
                case EnemyState.Searching:
                    cubeRenderer.material.color = Color.green; // Color verde para el estado de b�squeda
                    break;
                case EnemyState.Chasing:
                    cubeRenderer.material.color = Color.yellow; // Color amarillo para el estado de persecuci�n
                    break;
                case EnemyState.Attacking:
                    // Siempre cambia a rojo al atacar
                    cubeRenderer.material.color = Color.red; // Color rojo al atacar
                    break;
            }

            // Cambia a magenta si el ataque est� en cooldown
            if (currentState == EnemyState.Attacking && attackTimer > 0)
            {
                cubeRenderer.material.color = Color.magenta; // Color magenta durante el cooldown
            }
        }
        else
        {
            Debug.LogError("El cubo de estado no est� asignado en el inspector.");
        }
    }

}
    

