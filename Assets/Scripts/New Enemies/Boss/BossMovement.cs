using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : BossLIfe
{
    [Header("Jugador y Movimiento")]
    public Transform[] zonePoints; // Puntos fijos para las zonas
    private Transform player;      // Referencia al jugador
    public float moveSpeed = 5f;   // Velocidad de movimiento del Boss
    public float rotationSpeed = 5f; // Velocidad de rotaci�n

    private Vector3 currentTarget; // Punto al que se mueve el Boss
    
    [Header("Rangos de Distancia")]
    public float closeRange = 5f; // Distancia considerada cuerpo a cuerpo
    public float midRange = 15f; // Distancia considerada rango medio
    public float farRange = 25f; // Distancia considerada largo alcance

    [Header("Fases y Da�o")]
    public int phase = 1; // Fase del combate
  

    [Header("Detecci�n del Jugador")]
    private float behindTime = 0f; // Tiempo que el jugador ha pasado detr�s del jefe
    public float behindThreshold = 3f; // Tiempo necesario para el ataque de giro

    [Header("Prefabs y Arena")]
    public GameObject gearProjectile; // Prefab del engranaje cortante
    public GameObject gearTrapBot; // Prefab del robot trampa
    public GameObject gearTrapBot2; // Prefab del robot trampa
    public Transform[] arenaBounds; // L�mites de la arena para rebotar engranajes
    public Transform coreTransform; // Transform del n�cleo del jefe
    public GameObject barrido;
    public GameObject explosionPrefab; // Prefab de la explosi�n


    [Header("Cooldowns de Ataques")]
    private float lastSpinAttackTime = -10f; // Momento del �ltimo Spin Attack
    public float spinAttackCooldown = 5f;   // Tiempo entre Spin Attacks

    private float lastSweepingStrikeTime = -10f;
    public float sweepingStrikeCooldown = 8f;

    private float lastCoreBurstTime = -10f;
    public float coreBurstCooldown = 12f;

    private float lastGearTrapTime = -10f;
    public float gearTrapCooldown = 15f;

    protected override void Start()
    {
        base.Start();

        // Inicializa el objetivo al punto de la primera zona por defecto
        currentTarget = zonePoints[0].position;
        StartCoroutine(FindPlayerWithDelay());
    }

    private IEnumerator FindPlayerWithDelay()
    {
        yield return new WaitForSeconds(0.5f);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontr� un objeto con la etiqueta 'Player'");
        }
    }

    // Este m�todo es llamado cuando el jugador entra en una zona
    public void OnPlayerEnterZone(int zoneID)
    {
        if (zoneID >= 0 && zoneID < zonePoints.Length)
        {
            currentTarget = zonePoints[zoneID].position; // Actualiza el destino del Boss
        }
    }

    void Update()
    {
        MoveTowardsTarget(); // Mueve al Boss hacia el destino actual
        RotateTowardsPlayer(); // Ajusta la rotaci�n del Boss para mirar al jugador
        EvaluateConditions(); // Eval�a condiciones para ataques
        if (currentHealth <= maxHealth / 2 && phase == 1)
        {
            phase = 2;
            Debug.Log("entramos en fase2");
        }
    }

    private void MoveTowardsTarget()
    {
        // Mueve al Boss hacia el objetivo (sin NavMesh)
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);
    }

    void RotateTowardsPlayer()
    {
        // Calcula la direcci�n hacia el jugador
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Ignora la componente Y para que no se incline

        // Calcula la rotaci�n deseada para mirar al jugador
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // Aplica la rotaci�n de manera suave
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void EvaluateConditions()
    {
        Vector3 toPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, toPlayer);
        float distance = toPlayer.magnitude;

        // Fase 1
        if (phase == 1)
        {
            if (angle > 120f)
            {
                behindTime += Time.deltaTime;
                if (behindTime >= behindThreshold && Time.time > lastSpinAttackTime + spinAttackCooldown)
                {
                    CoreBurst();
                    behindTime = 0f;
                    lastSpinAttackTime = Time.time;
                }
            }
            else
            {
                behindTime = 0f;
            }

            if (distance > farRange && Time.time > lastCoreBurstTime + coreBurstCooldown)
            {
                SweepingStrike();
                lastCoreBurstTime = Time.time;
            }
            else if (distance < closeRange && Time.time > lastSweepingStrikeTime + sweepingStrikeCooldown)
            {
                SpinAttack();
                lastSweepingStrikeTime = Time.time;
            }

            if (phase == 1 && Time.time > lastGearTrapTime + gearTrapCooldown)
            {
                GearTrap();
                lastGearTrapTime = Time.time;
            }
        }

        // Fase 2
        if (phase == 2)
        {
            // Realizar el salto aplastante si el jugador est� detr�s durante 3 segundos
            if (angle > 120f)
            {
                behindTime += Time.deltaTime;
                if (behindTime >= behindThreshold && Time.time > lastSpinAttackTime + spinAttackCooldown)
                {

                    behindTime = 0f; // Reiniciamos el temporizador
                    lastSpinAttackTime = Time.time;
                }
            }
            else
            {
                behindTime = 0f; // Reiniciamos si el jugador no est� detr�s
            }

            // Llamada a la Tormenta de Engranajes
            if (Time.time > lastCoreBurstTime + coreBurstCooldown)
            {
                TormentaDeEngranajes(); // Realizamos la tormenta de engranajes
                lastCoreBurstTime = Time.time;
            }

            // Llamar a la descarga de orbes (attack)
            if (distance > farRange && Time.time > lastGearTrapTime + gearTrapCooldown)
            {
                OrbeAttack(); // Realizamos la descarga de orbes
                lastCoreBurstTime = Time.time;
            }
            else if (distance < closeRange && Time.time > lastSweepingStrikeTime + sweepingStrikeCooldown)
            {
                SpinAttack();
                lastSweepingStrikeTime = Time.time;
            }
        }
    }


    void SpinAttack()
    {
        Debug.Log("Realizando Spin Attack!");
        // Animaci�n y da�o en �rea
        GameObject barridoAtk = Instantiate(barrido, coreTransform.position, Quaternion.identity);
        barridoAtk.GetComponent<Barrido>().ExecuteSweep();

    }

    void SweepingStrike()
    {
        Debug.Log("Realizando Sweeping Strike!");
        // Animaci�n de barrido
        for (int i = 0; i < 3; i++)
        {
            Instantiate(gearProjectile, transform.position, Quaternion.identity);
        }
    }

    public void CoreBurst()
    {
       
        
            Debug.Log("Realizando Explosi�n Propia!");
            GameObject explosion = Instantiate(explosionPrefab, coreTransform.position, Quaternion.identity);

            // Llamar al m�todo TriggerExplosion del objeto reci�n instanciado
            explosion.GetComponent<ExplosionBoss>().TriggerExplosion();
        
    }

   void GearTrap()
    {
        Debug.Log("Realizando Trampa de Engranajes!");
        Instantiate(gearTrapBot, arenaBounds[0].position, Quaternion.identity);
        Instantiate(gearTrapBot2, arenaBounds[1].position, Quaternion.identity);
    }


    void OrbeAttack()
    {
        Debug.Log("Realizando Descarga de Orbes");
        // L�gica para liberar 10 orbes de energ�a alrededor del jefe
        // Se pueden instanciar los orbes y hacer que se muevan
    }

    // Salto Aplastante

    // Tormenta de Engranajes
    void TormentaDeEngranajes()
    {
        Debug.Log("Realizando Tormenta de Engranajes");
        // L�gica para lanzar m�ltiples engranajes desde el techo
    }
}