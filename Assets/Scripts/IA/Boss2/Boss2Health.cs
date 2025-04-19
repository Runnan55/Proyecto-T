using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Health : MonoBehaviour
{
    public BossSpawner[] bossSpawner;
    public GameObject meleeEnemyPrefab;

    private int currentPhase = 1;
    private int totalHealth = 100;
    private int currentHealth;

    private float checkInterval = 1f;
    private float nextCheckTime = 0f;


    [SerializeField] private BossArmController leftArm;
    [SerializeField] private BossArmController rightArm;
    private float palmCooldown = 4f;
    private float nextPalmTime;

    void Start()
    {
        currentHealth = totalHealth;
        SubscribeToObjects();
    }

    void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            EvaluateCombatBehavior();
        }

        if (currentPhase == 2 && Time.time >= nextPalmTime)
        {
            nextPalmTime = Time.time + palmCooldown;

            // Alternar entre los dos brazos (o usar ambos a la vez)
            if (Random.value > 0.5f)
                leftArm.TriggerPalmSmash();
            else
                rightArm.TriggerPalmSmash();
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
        Debug.Log(" ¡El boss ha muerto definitivamente!");
        Destroy(gameObject);
    }

    // === SISTEMA DE DECISIONES ===
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
            IncreaseSpawnRate(); // Simulado
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

    // Simulaciones de lógica

    private bool IsPlayerFar()
    {
        return Vector3.Distance(transform.position, GameObject.FindWithTag("Player").transform.position) > 30f;
    }

    private bool IsInBulletTime()
    {
        return Time.timeScale < 0.8f;
    }

    private bool PlayerUsedChargedAttackOnCore()
    {
        return false; // Aquí lo conectas con tu sistema real
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
        Debug.Log("Invocando barricada de enemigos melee por tiempo bala");

        foreach (var spawner in bossSpawner)
        {
            Instantiate(meleeEnemyPrefab, spawner.transform.position, Quaternion.identity);
        }
    }

    private void IncreaseSpawnRate()
    {
        Debug.Log("Aumentando velocidad de invocaciones (simulado)");
        // Conecta con tu sistema de invocaciones aquí
    }

    private bool BossHasArms()
    {
        return false; // Conecta con tu sistema de brazos
    }

    private void ProtectSelfWithArms()
    {
        Debug.Log("El boss se protege con los brazos");
        // Lógica de mover brazos a defender spawners
    }

    private void SummonMeleeWave()
    {
        Debug.Log("Invocando ola de enemigos melee (sin brazos ni spawners)");
        foreach (var spawner in bossSpawner)
        {
            Instantiate(meleeEnemyPrefab, spawner.transform.position, Quaternion.identity);
        }
    }
}
