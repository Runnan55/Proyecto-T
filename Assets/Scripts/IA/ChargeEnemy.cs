using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ChargeEnemy : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public float detectionRange = 10f; // Rango de detección del enemigo
    public float stoppingDistance = 10f; // Distancia a la que el enemigo se detiene del jugador
    public float chargeSpeed = 5f; // Velocidad de carga del embiste
    public float chargeDuration = 1f; // Duración de la carga del embiste
    public float cooldownDuration = 2f; // Duración del tiempo de espera entre embistes
    private NavMeshAgent agent; // Referencia al NavMeshAgent
    private bool playerDetected = false; // Flag para indicar si el jugador ha sido detectado
    private bool isCharging = false; // Flag para indicar si el enemigo está cargando

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        // Si el jugador es detectado
        if (PlayerDetected())
        {
            playerDetected = true;
            // Movemos al enemigo hacia el jugador
            MoveTowardsPlayer();
        }
        else
        {
            playerDetected = false;
        }
    }

    bool PlayerDetected()
    {
        // Calculamos la distancia entre el jugador y el enemigo
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si la distancia es menor o igual al rango de detección, retornamos true
        return distanceToPlayer <= detectionRange;
    }

    void MoveTowardsPlayer()
    {
        // Movemos al enemigo hacia el jugador
        agent.SetDestination(player.position);

        // Si la distancia entre el enemigo y el jugador es menor o igual a la distancia de parada,
        // detenemos al enemigo
        if (Vector3.Distance(transform.position, player.position) <= stoppingDistance)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (playerDetected && !isCharging)
            {
                isCharging = true;
                yield return Charge();
                yield return new WaitForSeconds(cooldownDuration);
            }
            yield return null;
        }
    }

    IEnumerator Charge()
    {
        // Calculamos la dirección hacia el jugador
        Vector3 direction = (player.position - transform.position).normalized;

        // Mantenemos al enemigo mirando hacia el jugador
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Deshabilitamos la navegación para que el enemigo no siga al jugador durante el embiste
        agent.enabled = false;

        // Cargamos hacia el jugador
        float elapsedTime = 0f;
        while (elapsedTime < chargeDuration)
        {
            transform.position += direction * chargeSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Habilitamos la navegación nuevamente
        agent.enabled = true;

        // Finalizamos la carga
        isCharging = false;
    }

    void OnDrawGizmosSelected()
    {
        // Dibujamos un gizmo visualizando el rango de detección del enemigo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}