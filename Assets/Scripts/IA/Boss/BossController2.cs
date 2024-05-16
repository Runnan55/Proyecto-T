using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossController2 : Enemy
{
    public GameObject punchPrefab;
    public GameObject[] circleObjects; // Array para los cinco c�rculos
    public float attackInterval = 2f; // Intervalo entre ataques
    public float punchRange = 2f;
    public float circleRange = 3f;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private float lastAttackTime;
    private bool isAttacking;

    public Image healthBar;
    private float maxHealth;

    void Awake()
    {
        maxHealth = health; 

        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastAttackTime = Time.time;
        isAttacking = false;

        // Desactivar los c�rculos al inicio
        foreach (GameObject circle in circleObjects)
        {
            circle.SetActive(false);
        }
    }

    void Update()
    {
        healthBar.fillAmount = health / maxHealth;

        if (player != null && !isAttacking)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Si ha pasado suficiente tiempo desde el �ltimo ataque
            if (Time.time - lastAttackTime > attackInterval)
            {
                Attack();
            }
            else if (distance > punchRange) // Si est� fuera del rango de ataque
            {
                // Calcular la direcci�n hacia el jugador
                Vector3 direction = (player.position - transform.position).normalized;
                Vector3 targetPosition = player.position - direction * 2f; // 1 unidad de distancia del jugador

                // Establecer la posici�n de destino del NavMeshAgent
                navMeshAgent.SetDestination(targetPosition);
            }
            else
            {
                // Si est� dentro del rango de ataque, detener al enemigo
                navMeshAgent.SetDestination(transform.position);
            }
        }
    }

    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        int attackType = Random.Range(0, 2); // Genera un n�mero aleatorio entre 0 y 1

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
        // L�gica de ataque de pu�etazo
        // Instancia el prefab del pu�etazo, aplica da�o, etc.
        Debug.Log("Punch!");

        // Calcular la direcci�n hacia el jugador
        Vector3 direction = (player.position - transform.position).normalized;

        // Instanciar los cubos del ataque de pu�etazo
        Instantiate(punchPrefab, transform.position + direction, Quaternion.identity);
        yield return new WaitForSeconds(1f); // Tiempo de espera para el ataque
        isAttacking = false;
    }

    IEnumerator CircleAttack()
    {
        // L�gica de ataque circular
        // Activar los c�rculos para mostrarlos durante el ataque
        foreach (GameObject circle in circleObjects)
        {
            circle.SetActive(true);
        }

        yield return new WaitForSeconds(2f); // Tiempo de espera para el ataque

        // Desactivar los c�rculos despu�s del ataque
        foreach (GameObject circle in circleObjects)
        {
            circle.SetActive(false);
        }

        isAttacking = false;
    }
}