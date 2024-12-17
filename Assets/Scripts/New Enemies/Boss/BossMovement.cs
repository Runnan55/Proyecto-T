using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : BossLIfe
{
    [Header("Jugador y Movimiento")]
    public Transform[] zonePoints; // Puntos fijos para las zonas
    private Transform player;      // Referencia al jugador
    public float moveSpeed = 5f;   // Velocidad de movimiento del Boss
    public float rotationSpeed = 5f; // Velocidad de rotación

    private Vector3 currentTarget; // Punto al que se mueve el Boss

    
    [Header("Rangos de Distancia")]
    public float closeRange = 5f; // Distancia considerada cuerpo a cuerpo
    public float midRange = 15f; // Distancia considerada rango medio
    public float farRange = 25f; // Distancia considerada largo alcance

    [Header("Fases y Daño")]
    public int phase = 1; // Fase del combate
  

    [Header("Detección del Jugador")]
    private float behindTime = 0f; // Tiempo que el jugador ha pasado detrás del jefe
    public float behindThreshold = 3f; // Tiempo necesario para el ataque de giro

    [Header("Prefabs y Arena")]
    public GameObject gearProjectile; // Prefab del engranaje cortante
    public Transform[] stormGearPositions;
    public GameObject gearTrapBot; // Prefab del robot trampa
    public GameObject gearTrapBot2; // Prefab del robot trampa
    public GameObject[] smokeTramps;
    public Transform[] arenaBounds; // Límites de la arena para rebotar engranajes
    public Transform coreTransform; // Transform del núcleo del jefe
    public GameObject barrido;
    public GameObject explosionPrefab; // Prefab de la explosión


    [Header("Cooldowns de Ataques")]
    private float lastExplosionTrasera = -10f; // Momento del último Spin Attack
    public float explosionTraseraCooldown = 5f;   // Tiempo entre Spin Attacks

    private float lastBarrido = -10f;
    public float barridoCooldown = 8f;

    private float lastTormenta = -10f;
    public float tormentaEngranaje = 6;

    private float lastDisparo1 = -10f;
    public float Disparo1 = 12f;

    private float lastTrampaRobot = -10f;
    public float TrampaRobotCooldown = 15f;

    public float orbeCooldown = 5f;  // Tiempo entre cada ataque de orbes
    private float lastOrbeAttackTime = -10f;  // Marca el último momento de invocación de orbes

    private int zonaActual=0;
    private bool saltos=false;

    [Header("Referencias a Ataques y Otros Componentes")]
    private BossJumpAttack jumpAttack; // Referencia al script de salto
    public OrbeAttackControler orbeAttackController;
    public GearStorm gearStorm;
    protected override void Start()
    {
        base.Start();

        // Inicializa el objetivo al punto de la primera zona por defecto
        currentTarget = zonePoints[0].position;
        StartCoroutine(FindPlayerWithDelay());
        jumpAttack = GetComponent<BossJumpAttack>();
    }

    private IEnumerator FindPlayerWithDelay()
    {
        yield return new WaitForSeconds(1f);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'");
        }
    }

    // Este método es llamado cuando el jugador entra en una zona
    public void OnPlayerEnterZone(int zoneID)
    {
        if (saltos)
        {
            if (zoneID == zonaActual)
            {
                if (zoneID >= 0 && zoneID < zonePoints.Length)
                {
                    currentTarget = zonePoints[zoneID].position; // Actualiza el destino del Boss
                    zonaActual = zoneID;
                    saltos = false;
                    
                        Debug.Log("trampas desactivas");
                    foreach (var trap in smokeTramps)
                    {
                        if (trap != null)
                        {
                            BigDamageZone damageZone = trap.GetComponent<BigDamageZone>();
                            if (damageZone != null)
                            {
                                damageZone.DeactivateDamage();
                            }
                        }
                    }

                }
            }
        }
        else
        {
            if (zoneID >= 0 && zoneID < zonePoints.Length)
            {
                currentTarget = zonePoints[zoneID].position; // Actualiza el destino del Boss
                zonaActual = zoneID;
            }
        }
       
    }

    void Update()
    {
        MoveTowardsTarget(); // Mueve al Boss hacia el destino actual
        RotateTowardsPlayer(); // Ajusta la rotación del Boss para mirar al jugador
        EvaluateConditions(); // Evalúa condiciones para ataques
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
        // Calcula la dirección hacia el jugador
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Ignora la componente Y para que no se incline

        // Calcula la rotación deseada para mirar al jugador
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // Aplica la rotación de manera suave
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime *MovimientoJugador.bulletTimeScale* rotationSpeed);
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
                if (contador>=5)
                {
                    CoreBurst();
                    contador = 0;
                }
            }
            else
            {
                behindTime = 0f;
            }

            if (distance > midRange && Time.time > lastDisparo1 + Disparo1)
            {
                ProyectilUnaDir();
                lastDisparo1 = Time.time;
            }
            else if (distance < closeRange && Time.time > lastBarrido + barridoCooldown)
            {
                Barrido();
                lastBarrido = Time.time;
            }

            if (phase == 1 && Time.time > lastTrampaRobot + TrampaRobotCooldown)
            {
                TrampaRobots();
                lastTrampaRobot = Time.time;
            }
        }

        // Fase 2
        if (phase == 2)
        {
            // Realizar el salto aplastante si el jugador está detrás durante 3 segundos
            if (angle > 120f)
            {
                behindTime += Time.deltaTime;
                if (behindTime >= behindThreshold && Time.time > lastExplosionTrasera + explosionTraseraCooldown)
                {
                    OnPlayerEnterZone(GetRandomIndexExcluding(zonaActual));                   
                    
                    jumpAttack.SaltoAplastante(currentTarget);

                    saltos = true;
                    StartCoroutine(cooldown());
                   
                    
                    behindTime = 0f; // Reiniciamos el temporizador
                    lastExplosionTrasera = Time.time;
                }
            }
            else
            {
                behindTime = 0f; // Reiniciamos si el jugador no está detrás
            }

            // Llamada a la Tormenta de Engranajes
            if (distance > midRange && Time.time > lastTormenta + tormentaEngranaje)
            {
                TormentaDeEngranajes(); // Realizamos la tormenta de engranajes
                lastTormenta = Time.time;
            }

            // Llamar a la descarga de orbes (attack)
            if (distance > midRange && Time.time > lastOrbeAttackTime + orbeCooldown)
            {
                orbeAttackController.PerformOrbeAttack(); // Llamamos al script OrbeAttackController
                lastOrbeAttackTime = Time.time;
            }
            else if (distance < closeRange && Time.time > lastBarrido + barridoCooldown)
            {
                Barrido();
                lastBarrido = Time.time;
            }
        }
    }

    private IEnumerator cooldown()
    {
        yield return new WaitForSeconds(3);  // Esperar 1 segundo.
       
        Debug.Log("trampas activas");
        foreach (var trap in smokeTramps)
        {
            if (trap != null)
            {
                BigDamageZone damageZone = trap.GetComponent<BigDamageZone>();
                if (damageZone != null)
                {
                    damageZone.ActivateDamage();
                }
            }
        }
    }

    public int GetRandomIndexExcluding(int excludedIndex)
    {
        // Obtener un índice aleatorio del rango total
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, zonePoints.Length);
        } while (randomIndex == excludedIndex);

        return randomIndex;
    }

    void Barrido()
    {
        Debug.Log("Realizando Spin Attack!");
        // Animación y daño en área
        GameObject barridoAtk = Instantiate(barrido, coreTransform.position, coreTransform.rotation);
        barridoAtk.GetComponent<Barrido>().ExecuteSweep();

    }

    void ProyectilUnaDir()
    {
        Debug.Log("Realizando Sweeping Strike!");
        // Animación de barrido
      
            GameObject proyectil = Instantiate(gearProjectile, coreTransform.position, Quaternion.identity);

            Vector3 direction = coreTransform.forward;
            proyectil.GetComponent<BouncingProyectil2>().Initialize(direction);
        
    }

    public void CoreBurst()
    {
       
        
            Debug.Log("Realizando Explosión Propia!");
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Llamar al método TriggerExplosion del objeto recién instanciado
            explosion.GetComponent<ExplosionBoss>().TriggerExplosion();
        
    }

   void TrampaRobots()
    {
        Debug.Log("Realizando Trampa de Engranajes!");
        Instantiate(gearTrapBot, arenaBounds[0].position, Quaternion.identity);
        Instantiate(gearTrapBot2, arenaBounds[1].position, Quaternion.identity);
    }



    // Tormenta de Engranajes
    void TormentaDeEngranajes()
    {
        if (gearStorm != null)
        {
            gearStorm.IniciarTormenta(transform.position);
        }
        else
        {
            Debug.LogError("GearStormController no asignado en el Boss.");
        }
    }


}