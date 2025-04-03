using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerEnemigos_Ia : EnemyLife
{
    #region Variables
    public float tiempoRevivir = 3f;
    public Mesh revivirMesh;
    private Mesh originalMesh;
    private MeshFilter meshFilter;
    public GameObject areaEffectPrefab;

    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;
    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform shootingPoint; // Punto desde donde se disparan los proyectiles
    public float projectileSpeed = 10f;
    public float spawnAreaRadius = 5f; // Radio del área donde caerán los proyectiles

    [Header("Shooting Settings")]
    public float shootingCooldown = 2f; // Tiempo ajustable entre ráfagas de disparos

    [Header("Reviving")]
    private bool isReviving = false;

    #endregion

    #region Unity Methods
    void Start()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter != null)
        {
            originalMesh = meshFilter.mesh;
        }

        // Iniciar la corrutina para disparar automáticamente
        StartCoroutine(AutoShootProjectiles());
    }

    void Update()
    {
       
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DisparoCargado>() != null)
        {
            antiRevivir = true;
            if (health <= 0)
            {
                StopAllCoroutines();
                ApplyAreaEffect();

                if (level != null)
                {
                    level.EnemyDefeated(this);
                }

                Destroy(gameObject, 0.2f);
            }
        }
    }
    #endregion

    #region Damage and Effects
    public override void CalcularDamage()
    {
        if (health <= 0)
        {
            if (antiRevivir)
            {
                ApplyAreaEffect();

                if (level != null)
                {
                    level.EnemyDefeated(this);
                }

                Destroy(gameObject, 0.2f);
            }
            else
            {
                if (!antiRevivir)
                {
                    StartCoroutine(DelayedRevivir());
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void ApplyAreaEffect()
    {
        if (areaEffectPrefab != null)
        {
            Instantiate(areaEffectPrefab, transform.position, Quaternion.identity);
        }
    }
    #endregion

    #region Reviving Mechanic

    private IEnumerator DelayedRevivir()
    {
        yield return null;
        if (revivir == true)
        {
            StartCoroutine(Revivir());
        }
        else
        {
            antiRevivir = true;
            ApplyAreaEffect();

            if (level != null)
            {
                level.EnemyDefeated(this);
            }

            Destroy(gameObject, 0.2f);
        }
        
    }

    IEnumerator Revivir()
    {
        if (antiRevivir) yield break;

        Debug.Log("reviviendo");
        isReviving = true;

        Collider collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        if (revivirMesh != null && meshFilter != null)
        {
            meshFilter.mesh = revivirMesh;
            yield return new WaitForSeconds(tiempoRevivir);
            meshFilter.mesh = originalMesh;
        }
        else
        {
            yield return new WaitForSeconds(tiempoRevivir);
        }
       
        if (collider != null) collider.enabled = true;

        health = maxHp;
        antiRevivir = false;
        isReviving = false;

        // Reanudar la generación de proyectiles después de revivir
        StartCoroutine(AutoShootProjectiles());
    }

    #endregion

    #region Shooting Logic
    private void ShootProjectile()
    {
        // Generar una posición aleatoria dentro de un área de 5f alrededor del shootingPoint
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnAreaRadius, spawnAreaRadius),
            0f, // Mantener la altura inicial del proyectil
            Random.Range(-spawnAreaRadius, spawnAreaRadius)
        );

        Vector3 spawnPosition = shootingPoint.position + randomOffset;

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (spawnPosition - shootingPoint.position).normalized;
            rb.velocity = direction * projectileSpeed;
        }
    }

    private IEnumerator AutoShootProjectiles()
    {
        while (true)
        {
            // Detener la generación de proyectiles si está reviviendo
            if (isReviving)
            {
                yield return null;
                continue;
            }

            int projectilesToShoot = Random.Range(1, 4); // Número aleatorio entre 1 y 3

            for (int i = 0; i < projectilesToShoot; i++)
            {
                ShootProjectile(); // Instanciar un proyectil
                yield return new WaitForSeconds(0.2f); // Pequeño retraso entre disparos consecutivos
            }

            yield return new WaitForSeconds(shootingCooldown); // Esperar antes de la siguiente ráfaga
        }
    }
    #endregion
}
