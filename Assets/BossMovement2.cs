using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement2 : BossLIfe
{
    // === Player & Movement ===
    [Header("Jugador y Movimiento")]
    private Transform player;
    private Rigidbody rb;
    private Vector3 currentTarget; // Punto al que se mueve el Boss
    private bool canMove = true;



    [Header("Material del Jefe")]
    public Renderer bossRenderer;
    public Color fase1Color = Color.red;
    public Color fase2Color = Color.blue;

    // === Movement Settings ===
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public Transform[] zonePoints;
    private int zonaActual = 0;

    // === Action Timers ===
    [Header("Temporizadores de Acción")]
    private float actionTimer;
    public float stopDuration = 1.5f;
    public float moveDuration = 4f;

    // === Attack Settings ===
    [Header("Configuración de Ataque")]
    public float attackRange = 5f; // Rango para detectar si el jugador está cerca
    private bool isAttacking;
    private bool inCloseRange;

    // === Cooldown Settings ===
    [Header("Configuración de Cooldown")]
    public float cooldown = 1.5f;
    private float lastCooldown = -10;

    // === Boss Phase ===
    [Header("Fase del Jefe")]
    public int fase = 1;

    // === Abilities & Attacks ===
    [Header("Habilidades y Ataques")]
    public GearStorm gearStorm;
    public OrbeAttackControler orbeAttackController;
    private BossJumpAttack jumpAttack;
    private bool saltos = false;

    // === Traps & Explosions ===
    [Header("Trampas y Explosiones")]
    public GameObject explosionPrefab;
    public GameObject explosionPrefab2;
    public GameObject barrido;
    public GameObject barrido2;
    public GameObject engranaje;
    public GameObject gearTrapBot; // Prefab del robot trampa
    public GameObject gearTrapBot2; // Prefab del robot trampa
    public GameObject[] smokeTramps;

    // === Arena & Environment ===
    [Header("Arena y Entorno")]
    public Transform coreTransform;
    public Transform[] arenaBounds; // Límites de la arena para rebotar engranajes
    private bool trampasCanceladas = false;

    private Animator animator;
    

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        actionTimer = moveDuration;
        isAttacking = false;
        inCloseRange = false;
        StartCoroutine(FindPlayerWithDelay());
        jumpAttack = GetComponent<BossJumpAttack>();
        currentTarget = zonePoints[0].position;
        animator = GetComponent<Animator>();

        if (bossRenderer != null)
        {
            bossRenderer.material.color = fase1Color;
        }

    }

    private IEnumerator FindPlayerWithDelay()
    {
        yield return new WaitForSeconds(1f);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontr� un objeto con la etiqueta 'Player'");
        }
    }

    protected override void Update()
    {
        base.Update();
        HandleMovement();
        RotateTowardsPlayer();
        
        if (currentHealth <= maxHealth / 2 && fase == 1)
        {
            fase = 2;
            ExplosionPropia();
            Debug.Log("entramos en fase2");

              if (bossRenderer != null)
        {
            bossRenderer.material.color = fase2Color;
        }
        }
    }

    public void OnPlayerEnterZone(int zoneID)
    {
        if (saltos)
        {
            if (zoneID == zonaActual)
            {
                currentTarget = zonePoints[zoneID].position;
                zonaActual = zoneID;
                saltos = false;
                canMove = true;

                trampasCanceladas = true; // Si el jugador entra en la zona, cancelamos trampas

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
        else
        {
            if (zoneID >= 0 && zoneID < zonePoints.Length)
            {
                currentTarget = zonePoints[zoneID].position;
                zonaActual = zoneID;
            }
        }
    }

    private IEnumerator cooldown2()
    {
        yield return new WaitForSeconds(3);  // Esperar 1 segundo.

        //Debug.Log("trampas activas");
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
        // Obtener un �ndice aleatorio del rango total
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, zonePoints.Length);
        } while (randomIndex == excludedIndex);

        return randomIndex;
    }

    void HandleMovement()
    {
        if (!canMove)
        {
            animator.SetBool("IsMoving", false);
            rb.velocity = Vector3.zero;
            return;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            rb.velocity = Vector3.zero;
            PerformCloseRangeAttack();
            inCloseRange = true;
            return;
        }
        else if (inCloseRange)
        {
            // Reiniciar el temporizador al salir del rango de ataque cuerpo a cuerpo
            actionTimer = moveDuration;
            inCloseRange = false;
        }

        actionTimer -= Time.deltaTime;

        if (isAttacking)
        {
            animator.SetBool("IsMoving", false);
            rb.velocity = Vector3.zero;
            if (actionTimer <= 0)
            {
                isAttacking = false;
                actionTimer = moveDuration;
            }
        }
        else
        {
            animator.SetBool("IsMoving", true);
            Vector3 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed * MovimientoJugador.bulletTimeScale;

            if (actionTimer <= 0)
            {
                isAttacking = true;
                actionTimer = stopDuration;
                PerformRangedAttack();
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed * MovimientoJugador.bulletTimeScale);
    }

    void PerformRangedAttack()
    {
        if (fase == 1)
        {
            if (Random.Range(0f, 1f) > 0.5f && Time.time > lastCooldown + cooldown)
            {
                Engranaje();
                lastCooldown = Time.time;
                Debug.Log("atck");

            }
            else if (Time.time > lastCooldown + cooldown)
            {
                TrampaRobots();
                lastCooldown = Time.time;
                Debug.Log("atck");

            }
        }

        if (fase == 2)
        {
            if (Random.Range(0f, 1f) > 0.5f && Time.time > lastCooldown + cooldown)
            {
                TormentaDeEngranajes();
                lastCooldown = Time.time;
                Debug.Log("atckd");

            }
            else if (Time.time > lastCooldown + cooldown)
            {
                ExplosionEngranajes();
                lastCooldown = Time.time;
                Debug.Log("atckd");

            }
        }
    }

    void PerformCloseRangeAttack()
    {
        Debug.Log("Boss realiza un ataque cuerpo a cuerpo.");
       if (fase == 1)
        {
            if (contador>=3 && Time.time > lastCooldown + cooldown)
            {
                ExplosionPropia();
                lastCooldown = Time.time;
                Debug.Log("atckd");
                contador = 0;

            }
            else if (Time.time > lastCooldown + cooldown)
            {
                Barrido();
                lastCooldown = Time.time;
                Debug.Log("atckd");

            }
        }

       if (fase == 2)
        {
            if (Random.Range(0f, 1f) > 0.5f && Time.time > lastCooldown + cooldown)
            {
                Barrido2();
                lastCooldown = Time.time;
                Debug.Log("atckd");

            }
            else if (contador >= 8 && Time.time > lastCooldown + cooldown)
            {
                Salto();
                lastCooldown = Time.time;
                Debug.Log("gdgege");
                contador = 0;

            }
            if ( contador2 >= 6 && Time.time > lastCooldown + cooldown)
            {
                ExplosionPropia2();
                lastCooldown = Time.time;
                Debug.Log("atckd");
                contador2 = 0;
            }
        }
       
    }

    public void Barrido()
    {
        animator.SetTrigger("Attack_Barrido");

        GameObject barridoAtk = Instantiate(barrido, coreTransform.position, coreTransform.rotation);
        barridoAtk.GetComponent<Barrido>().ExecuteSweep();
    }
    public void Barrido2()
    {
        animator.SetTrigger("Attack_Barrido");

        GameObject barridoAtk = Instantiate(barrido2, coreTransform.position, coreTransform.rotation);
        barridoAtk.GetComponent<Barrido>().ExecuteSweep();
    }
    public void ExplosionPropia()
    {
        animator.SetTrigger("Attack_Explosion");
        Debug.Log("pumm");
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponent<ExplosionBoss>().TriggerExplosion();
    }
    public void ExplosionPropia2()
    {
        animator.SetTrigger("Attack_Explosion");
        Debug.Log("pumm");
        GameObject explosion = Instantiate(explosionPrefab2, transform.position, Quaternion.identity);
        explosion.GetComponent<ExplosionBoss>().TriggerExplosion();
    }
    public void Engranaje()
    {
        animator.SetTrigger("Attack_Disparo");
        GameObject proyectil = Instantiate(engranaje, coreTransform.position, Quaternion.identity);

        Vector3 direction = coreTransform.forward;
        proyectil.GetComponent<BouncingProyectil2>().Initialize(direction);

    }
    public void TrampaRobots()
    {
        animator.SetTrigger("Attack_MiniBots");

        Instantiate(gearTrapBot, arenaBounds[0].position, Quaternion.identity);
        Instantiate(gearTrapBot2, arenaBounds[1].position, Quaternion.identity);
    }
    public void Salto()
    {
        animator.SetTrigger("Attack_Salto");
        int nuevaZona = GetRandomIndexExcluding(zonaActual);
        int zonaAnterior = zonaActual; // Guardamos la zona actual antes de cambiarla
        trampasCanceladas = false; // Reiniciamos la variable cuando el jefe salta

        OnPlayerEnterZone(nuevaZona);
        jumpAttack.SaltoAplastante(currentTarget);
        saltos = true;
        canMove = false;

        // Activar trampas en tiempos programados, pero solo si no se han cancelado
        StartCoroutine(ActivarTrampaAnterior(zonaAnterior));
        StartCoroutine(ActivarOtrasTrampas(zonaAnterior, nuevaZona));
    }

    private IEnumerator ActivarTrampaAnterior(int zonaID)
    {
        yield return new WaitForSeconds(1);

        if (!trampasCanceladas && zonaID >= 0 && zonaID < smokeTramps.Length && smokeTramps[zonaID] != null)
        {
            BigDamageZone damageZone = smokeTramps[zonaID].GetComponent<BigDamageZone>();
            if (damageZone != null)
            {
                damageZone.ActivateDamage();
            }
        }
    }

    private IEnumerator ActivarOtrasTrampas(int zonaExcluida, int zonaAterrizaje)
    {
        yield return new WaitForSeconds(3);

        if (trampasCanceladas) yield break; // Si las trampas fueron canceladas, detenemos la ejecución

        for (int i = 0; i < smokeTramps.Length; i++)
        {
            if (i != zonaExcluida && i != zonaAterrizaje && smokeTramps[i] != null)
            {
                BigDamageZone damageZone = smokeTramps[i].GetComponent<BigDamageZone>();
                if (damageZone != null)
                {
                    damageZone.ActivateDamage();
                }
            }
        }
    }


    public void TormentaDeEngranajes()
    {
        animator.SetTrigger("Attack_Tuerca");

        
        if (gearStorm != null)
        {
            gearStorm.IniciarTormenta(transform.position);
        }
        else
        {
            Debug.LogError("GearStormController no asignado en el Boss.");
        }
    }
    public void ExplosionEngranajes()
    {
        animator.SetTrigger("Attack_DisparoAmplio");

        orbeAttackController.PerformOrbeAttack();
    }




}