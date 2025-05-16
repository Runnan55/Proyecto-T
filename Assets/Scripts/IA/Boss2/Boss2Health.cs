using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Para actualizar la barra de vida

public class Boss2Health : MonoBehaviour
{
    [Header("Spawners y enemigos")]
    public BossSpawner[] bossSpawner;
    public GameObject meleeEnemyPrefab;

    [Header("Ataques de Fase 2")]
    public PalmadaAttack palmadaAttack;
    public PunoTaladro punoTaladro;
    public BarridoAttack barridoAttack;

    [Header("Puntos de ataque")]
    public Transform palmPointA;
    public Transform palmPointB;
    public Transform punchPointA;
    public Transform punchPointB;
    public Transform sweepStart;
    public Transform sweepEnd;

    [Header("Orbes ralentizadores")]
    public GameObject orbPrefab;
    public Transform orbSpawnPoint;
    public float orbInterval = 8f;
    private float nextOrbTime = 0f;

    [Header("Brazos del boss")]
    public BossArmProtector leftArm;
    public BossArmProtector rightArm;

    public int currentPhase = 1;
    private int totalHealth = 100;
    private int currentHealth;

    private float checkInterval = 1f;
    private float nextCheckTime = 0f;

    private float attackInterval = 6f;
    private float nextAttackTime = 0f;

    private float respawnInterval = 20f;  // Intervalo para regenerar los spawners después de ser destruidos
    private float nextRespawnTime = 0f;

    public Image healthBar;


    void Start()
    {
        currentHealth = totalHealth;
        SubscribeToObjects();

    }

    void Update()
    {

        // Comportamientos generales (IA)
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
        }

        // Ataques de Fase 2
        if (currentPhase == 2 && Time.time >= nextAttackTime)
        {
            ExecuteRandomAttack();
            nextAttackTime = Time.time + attackInterval;
        }

        if ((currentPhase == 1 || currentPhase == 2) && Time.time >= nextOrbTime)
        {
            nextOrbTime = Time.time + orbInterval;
            LaunchSlowOrb();
        }

        if (Input.GetKeyDown(KeyCode.L) && currentPhase == 2)
        {
            TriggerArmProtection();
        }

        // Reaparecer los spawners por tiempo en ambas fases
        RegenerateSpawners();
    }

    void SubscribeToObjects()
    {
        foreach (var obj in bossSpawner)
        {
            obj.OnDestroyed += HandleObjectDestroyed;
        }
    }

    private void HandleObjectDestroyed(BossSpawner destroyedObj)
    {
        if (AllObjectsDestroyed())
        {
            if (currentPhase == 1)
            {
                StartPhaseTwo();
            }
            else
            {
                Die();
            }
        }
    }

    private bool AllObjectsDestroyed()
    {
        foreach (var obj in bossSpawner)
        {
            if (!obj.IsDestroyed())
                return false;
        }
        return true;
    }

    private void StartPhaseTwo()
    {
        Debug.Log("Fase 2 activada");
        currentPhase = 2;
        currentHealth = totalHealth / 2;

        // Establecemos el tiempo de regeneración de los spawners
        nextRespawnTime = Time.time + respawnInterval;

        // Llamamos a la corutina para regenerar los spawners después de un retardo de 1 segundo
        StartCoroutine(RegenerateSpawnersWithDelay(1f));  // 1 segundo de retardo
    }

    // Corutina para añadir un retardo antes de regenerar los spawners
    private IEnumerator RegenerateSpawnersWithDelay(float delay)
    {
        // Espera el tiempo especificado (1 segundo en este caso)
        yield return new WaitForSeconds(delay);

        // Ahora regeneramos todos los spawners
        RegenerateAllSpawners();
    }

    // Regeneración de todos los spawners
    private void RegenerateAllSpawners()
    {
        foreach (var spawner in bossSpawner)
        {
            spawner.ResetObject();  // Regeneramos cada spawner
        }

        Debug.Log("Spawners regenerados en fase 2");
    }

    private void Die()
    {
        Debug.Log("¡El boss ha muerto definitivamente!");
        Destroy(gameObject);
    }

    private void RegenerateSpawners()
    {
        foreach (var spawner in bossSpawner)
        {
            // En ambas fases, si un spawner está destruido y ha pasado el tiempo de regeneración, lo regeneramos
            if (spawner.IsDestroyed() && Time.time >= nextRespawnTime)
            {
                spawner.ResetObject(); // Regeneramos el spawner individualmente
                nextRespawnTime = Time.time + respawnInterval; // Actualizamos el tiempo de regeneración
                Debug.Log("Spawner regenerado.");
            }
        }
    }


    private void ExecuteRandomAttack()
    {
        int random = Random.Range(0, 3);

        if (random == 0)
        {
            palmadaAttack.ExecutePalmada(palmPointA, palmPointB);
        }
        else if (random == 1)
        {
            punoTaladro.ExecutePunetazo(punchPointA, punchPointB);
        }
        else
        {
            barridoAttack.ExecuteBarrido(sweepStart, sweepEnd);
        }

        Debug.Log(" Ataque lanzado: " + random);
    }

    private void LaunchSlowOrb()
    {
        if (orbPrefab != null && orbSpawnPoint != null)
        {
            Instantiate(orbPrefab, orbSpawnPoint.position, Quaternion.identity);
            Debug.Log("Orbe ralentizador lanzado desde el jefe");
        }
    }

    public void TriggerArmProtection()
    {
        List<IProtectable> protectableTargets = new List<IProtectable>();

        foreach (var spawner in bossSpawner)
        {
            if (!spawner.IsDestroyed())
            {
                protectableTargets.Add(spawner);
            }
        }

        // Si no hay spawners activos, el jefe se vuelve el único objetivo protegible
        if (protectableTargets.Count == 0)
        {
            protectableTargets.Add(this as IProtectable); // el propio jefe
            protectableTargets.Add(this as IProtectable); // para que ambos brazos lo protejan
        }

        if (protectableTargets.Count > 0)
            leftArm.Protect(protectableTargets[0]);

        if (protectableTargets.Count > 1)
            rightArm.Protect(protectableTargets[1]);
    }
}
