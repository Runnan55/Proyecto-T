using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController2 : Enemy
{
    public GameObject punchPrefab;
    public GameObject[] circleObjects; // Array para los cinco círculos
    public float punchCooldown = 3f;
    public float circleCooldown = 5f;
    public float punchRange = 2f;
    public float circleRange = 3f;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private float lastAttackTime;
    private bool isAttacking;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastAttackTime = Time.time;
        isAttacking = false;

        // Desactivar los círculos al inicio
        foreach (GameObject circle in circleObjects)
        {
            circle.SetActive(false);
        }
    }

    void Update()
    {
        if (player != null && !isAttacking)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Si está lo suficientemente cerca y ha pasado suficiente tiempo desde el último ataque
            if (Time.time - lastAttackTime > punchCooldown && distance < punchRange)
            {
                Attack();
            }
            else if (distance > punchRange) // Si está fuera del rango de ataque
            {
                // Calcular la dirección hacia el jugador
                Vector3 direction = (player.position - transform.position).normalized;
                Vector3 targetPosition = player.position - direction * 1f; // 1 unidad de distancia del jugador

                // Establecer la posición de destino del NavMeshAgent
                navMeshAgent.SetDestination(targetPosition);
            }
            else
            {
                // Si está dentro del rango de ataque, detener al enemigo
                navMeshAgent.SetDestination(transform.position);
            }
        }
    
    }

    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        int attackType = Random.Range(0, 2); // Genera un número aleatorio entre 0 y 1

        switch (attackType)
        {
            case 0:
                StartCoroutine(Punch());
                break;
            case 1:
                StartCoroutine(CircleAttack());
                break;
            default:
                break;
        }
    }

    IEnumerator Punch()
    {
        // Lógica de ataque de puñetazo
        // Instancia el prefab del puñetazo, aplica daño, etc.
        Debug.Log("Punch!");

        // Calcular la dirección hacia el jugador
        Vector3 direction = (player.position - transform.position).normalized;

        // Instanciar los cubos del ataque de puñetazo
        Instantiate(punchPrefab, transform.position + direction, Quaternion.identity);
        yield return new WaitForSeconds(1f); // Tiempo de espera para el ataque
        isAttacking = false;
    }

    IEnumerator CircleAttack()
    {
        // Lógica de ataque circular
        // Activar los círculos para mostrarlos durante el ataque
        foreach (GameObject circle in circleObjects)
        {
            circle.SetActive(true);
        }

        yield return new WaitForSeconds(2f); // Tiempo de espera para el ataque

        // Desactivar los círculos después del ataque
        foreach (GameObject circle in circleObjects)
        {
            circle.SetActive(false);
        }

        isAttacking = false;
    }
}