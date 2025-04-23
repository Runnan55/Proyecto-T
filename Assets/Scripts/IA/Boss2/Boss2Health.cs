using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            EvaluateCombatBehavior();
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
        StartCoroutine(RespawnSpawnersWithDelay(1f));
        nextAttackTime = Time.time + 2f;
    }

    private IEnumerator RespawnSpawnersWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var spawner in bossSpawner)
        {
            spawner.ResetObject();
        }

        Debug.Log("Spawners reaparecidos en Fase 2");
    }

    private void Die()
    {
        Debug.Log("¡El boss ha muerto definitivamente!");
        Destroy(gameObject);
    }

    // ===================
    // COMPORTAMIENTO IA
    // ===================
    private void EvaluateCombatBehavior()
    {
        if (IsPlayerFar())
        {
            TryRespawnMissingSpawners();
        }

        if (IsInBulletTime())
        {
            SpawnMeleeBarricade();
        }

        if (PlayerUsedChargedAttackOnCore())
        {
            IncreaseSpawnRate(); // aún simulado
        }

        if (currentPhase == 2 && AllObjectsDestroyed())
        {
            if (BossHasArms())
            {
                ProtectSelfWithArms();
            }
            else
            {
                SummonMeleeWave();
            }
        }
    }

    private bool IsPlayerFar()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return false;

        return Vector3.Distance(transform.position, player.transform.position) > 15f;
    }

    private bool IsInBulletTime()
    {
        return Time.timeScale < 0.8f;
    }

    private bool PlayerUsedChargedAttackOnCore()
    {
        return false; // conectar con tu sistema real
    }

    private void TryRespawnMissingSpawners()
    {
        foreach (var spawner in bossSpawner)
        {
            if (spawner.IsDestroyed())
            {
                Debug.Log("Reapareciendo spawner por distancia del jugador");
                spawner.ResetObject();
                return;
            }
        }
    }

    private void SpawnMeleeBarricade()
    {
        Debug.Log("Invocando enemigos melee por tiempo bala");
        foreach (var spawner in bossSpawner)
        {
            Instantiate(meleeEnemyPrefab, spawner.transform.position, Quaternion.identity);
        }
    }

    private void IncreaseSpawnRate()
    {
        Debug.Log("Aumentando velocidad de spawneo (simulado)");
        // Aquí conectarías con la lógica de tus spawners
    }

    private bool BossHasArms()
    {
        return false; // conectar con tus brazos reales
    }

    private void ProtectSelfWithArms()
    {
        Debug.Log("El boss se protege con los brazos");
        // Animaciones / lógica de protección real
    }

    private void SummonMeleeWave()
    {
        Debug.Log("Ola de enemigos por no tener brazos ni spawners");
        foreach (var spawner in bossSpawner)
        {
            Instantiate(meleeEnemyPrefab, spawner.transform.position, Quaternion.identity);
        }
    }

    // ===================
    // ATAQUES FASE 2
    // ===================
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