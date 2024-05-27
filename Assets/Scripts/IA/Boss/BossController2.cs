using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossController2 : BossHealth
{
    public GameObject enemyPrefab; // Prefab del enemigo invocado
    public Transform[] spawnPoints; // Puntos de spawn para los enemigos invocados
    public GameObject[] circleObjects; // Array para los cinco círculos
    public GameObject fireballPrefab; // Prefab de la bola de fuego
    public Transform[] fireballSpawnPoints; // Array de puntos de spawn para las bolas de fuego
    public float fireballSpeed = 10f; // Velocidad de las bolas de fuego
    public Color invulnerableColor = Color.red; // Color del jefe cuando es invulnerable
    public float invulnerableDuration = 5f; // Duración del estado invulnerable
    public float attackInterval = 2f; // Intervalo entre ataques
    public float punchRange = 2f;
    public float circleRange = 3f;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private float lastAttackTime;
    private bool isAttacking;
    private bool isInvulnerable = false;
    private Renderer bossRenderer;
    private Color originalColor;

    public Image healthBar;

    void Awake()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastAttackTime = Time.time;
        isAttacking = false;

        bossRenderer = GetComponent<Renderer>();
        originalColor = bossRenderer.material.color;

        // Desactivar los círculos al inicio
        foreach (GameObject circle in circleObjects)
        {
            circle.SetActive(false);
        }
    }

    void Update()
    {
        healthBar.fillAmount = currentHealth / maxHealth;

        if (player != null && !isAttacking)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Si ha pasado suficiente tiempo desde el último ataque
            if (Time.time - lastAttackTime > attackInterval)
            {
                Attack();
            }
            else if (distance > punchRange) // Si está fuera del rango de ataque
            {
                // Calcular la dirección hacia el jugador
                Vector3 direction = (player.position - transform.position).normalized;
                Vector3 targetPosition = player.position - direction * 2f; // 1 unidad de distancia del jugador

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

        int attackType = Random.Range(0, 3); // Genera un número aleatorio entre 0 y 2

        switch (attackType)
        {
            case 0:
                StartCoroutine(InvulnerabilityAttack());
                break;
            case 1:
                StartCoroutine(CircleAttack());
                break;
            case 2:
                StartCoroutine(FireballAttack());
                break;
            default:
                break;
        }
    }

    IEnumerator InvulnerabilityAttack()
    {
        // Lógica de ataque de invulnerabilidad
        Debug.Log("Invulnerability Attack!");

        isInvulnerable = true;
        bossRenderer.material.color = invulnerableColor;
        navMeshAgent.isStopped = true;

        // Invocar enemigos durante la invulnerabilidad
        float spawnInterval = invulnerableDuration / spawnPoints.Length;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Instantiate(enemyPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitForSeconds(invulnerableDuration);

        isInvulnerable = false;
        bossRenderer.material.color = originalColor;
        navMeshAgent.isStopped = false;

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

    IEnumerator FireballAttack()
    {
        // Lógica de ataque de bolas de fuego
        Debug.Log("Fireball Attack!");

        foreach (Transform spawnPoint in fireballSpawnPoints)
        {
            GameObject fireball = Instantiate(fireballPrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = fireball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = spawnPoint.forward * fireballSpeed;
            }
        }

        yield return new WaitForSeconds(1f); // Tiempo de espera para el ataque
        isAttacking = false;
    }

    public override void TakeDamage(float amount)
    {
        if (!isInvulnerable)
        {
            base.TakeDamage(amount);
        }
    }
}