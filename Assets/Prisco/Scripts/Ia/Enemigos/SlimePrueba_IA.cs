using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Para usar funciones de LINQ
using UnityEngine;
using UnityEngine.AI;

public class SlimePrueba_IA : EnemyLife
{
    #region Variables  
    private Mesh originalMesh;
    private MeshFilter meshFilter; 
    public GameObject areaEffectPrefab;   
    public GameObject slimeOriginalPrefab; // Prefab del slime original
    public GameObject slimeCopiaPrefab; // Prefab del slime copia
    public Transform jugador; // Referencia al jugador
    public float velocidad = 3f; // Velocidad de movimiento
    private Color colorInicial;
    public Color colorFinal = Color.red;
    public float tiempoCambioColor = 1f;
    private Renderer rendererEsqueleto;
    private bool activandoCollider = false;
    public Transform puntoAtaque; // Objeto vacío donde se instanciará el prefab del ataque
    public GameObject prefabAtaque; // Prefab del ataque
    public float rangoAtaque = 2f; // Rango total para que el slime ataque
    private NavMeshAgent navMeshAgent; // Referencia al NavMeshAgent
    private bool enSalto = false; // Controla si el slime está en medio de un salto
    public float alturaSalto = 1f; // Altura del salto
    public float duracionSalto = 0.5f; // Duración de cada salto
    public float distanciaSalto = 1f; // Distancia de cada salto
    public bool tieneCorazon = false; // Indica si este slime tiene el corazón
    public int grupoID; // Identificador único del grupo al que pertenece este slime
    private static Dictionary<int, List<SlimePrueba_IA>> gruposSlimes = new Dictionary<int, List<SlimePrueba_IA>>(); // Diccionario de grupos de slimes
    private static int siguienteGrupoID = 1; // Controla el próximo ID único para los grupos
    public bool esSlimeV2 = false; // Indica si este slime es el prefab especial SlimeV2

    private enum Estado { Chasing, Attacking }
    private Estado estadoActual = Estado.Chasing;
    private float tiempoEsperaAtaque = 1f;
    #endregion

    #region Unity Methods
    void Start()
    {
        meshFilter = GetComponentInChildren<MeshFilter>(); 
        if (meshFilter != null)
        {
            originalMesh = meshFilter.mesh; 
        }

        rendererEsqueleto = GetComponentInChildren<Renderer>();
        if (rendererEsqueleto != null)
        {
            colorInicial = rendererEsqueleto.material.color; // Guardar el color original dinámicamente
        }

        if (jugador == null)
        {
            jugador = GameObject.FindGameObjectWithTag("Player").transform; // Buscar al jugador por tag
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = velocidad; // Configurar la velocidad del agente
        }

        // Si el grupoID no está asignado, asignar un nuevo grupoID
        if (grupoID == 0)
        {
            grupoID = siguienteGrupoID++;
        }

        // Asegurarse de que el grupo existe en el diccionario
        if (!gruposSlimes.ContainsKey(grupoID))
        {
            gruposSlimes[grupoID] = new List<SlimePrueba_IA>();
        }

        // Agregar este slime al grupo correspondiente
        if (!gruposSlimes[grupoID].Contains(this))
        {
            gruposSlimes[grupoID].Add(this);
        }

        Debug.Log($"Slime registrado en el grupo {grupoID}. Total en el grupo: {gruposSlimes[grupoID].Count}");

        // Verificar si este slime es el prefab especial SlimeV2
        if (gameObject.name.Contains("SlimeV2"))
        {
            esSlimeV2 = true;
        }
    }

    void Update()
    {
        switch (estadoActual)
        {
            case Estado.Chasing:
                if (!activandoCollider && jugador != null)
                {
                    MoverHaciaJugador();
                }
                break;

            case Estado.Attacking:
                // El ataque se maneja en la corrutina, no es necesario hacer nada aquí.
                break;
        }

        // Hacer que el enemigo mire al jugador todo el tiempo
        if (jugador != null)
        {
            Vector3 direccion = (jugador.position - transform.position).normalized;
            Quaternion rotacion = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, Time.deltaTime * 5f); // Suavizar la rotación
        }
    }

    void OnDestroy()
    {
        // Remover este slime del grupo al ser destruido
        if (gruposSlimes.ContainsKey(grupoID))
        {
            gruposSlimes[grupoID].Remove(this);
            Debug.Log($"Slime eliminado del grupo {grupoID}. Restantes en el grupo: {gruposSlimes[grupoID].Count}");

            if (gruposSlimes[grupoID].Count == 0)
            {
                gruposSlimes.Remove(grupoID); // Eliminar el grupo si está vacío
                Debug.Log($"Grupo {grupoID} eliminado.");
            }
        }
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
            }

            // Solo destruir el grupo si este slime es el prefab especial SlimeV2
            if (esSlimeV2)
            {
                Debug.Log($"El slime especial SlimeV2 del grupo {grupoID} ha muerto. Destruyendo el grupo...");
                DestruirGrupoSlimes(); // Destruir todos los slimes del grupo
            }

            if (level != null)
            {
                level.EnemyDefeated(this);
            }

            StartCoroutine(ReducirEscalaYDestruir()); // Reducir escala antes de destruir
        }
    }

    private IEnumerator ReducirEscalaYDestruir()
    {
        float tiempo = 0f;
        float duracion = 1f; // Duración de la reducción de escala
        Vector3 escalaInicial = transform.localScale;
        Vector3 escalaFinal = Vector3.zero;

        // Reducir la escala gradualmente
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, tiempo / duracion);
            yield return null;
        }

        // Instanciar el slime original y una copia después de reducir la escala
        if (slimeOriginalPrefab != null && slimeCopiaPrefab != null)
        {
            var slimeOriginal = Instantiate(slimeOriginalPrefab, transform.position + Vector3.right, Quaternion.identity);
            var slimeCopia = Instantiate(slimeCopiaPrefab, transform.position + Vector3.left, Quaternion.identity);

            // Asignar el grupo y el corazón al slime original si este slime lo tenía
            var slimeOriginalIA = slimeOriginal.GetComponent<SlimePrueba_IA>();
            var slimeCopiaIA = slimeCopia.GetComponent<SlimePrueba_IA>();

            if (slimeOriginalIA != null)
            {
                slimeOriginalIA.grupoID = grupoID; // Heredar el grupoID del slime padre
               
            }

            if (slimeCopiaIA != null)
            {
                slimeCopiaIA.grupoID = grupoID; // Heredar el grupoID del slime padre
                
            }
        }

        // Esperar 1 segundo antes de destruir
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void ApplyAreaEffect()
    {
        if (areaEffectPrefab != null)
        {
            Instantiate(areaEffectPrefab, transform.position, Quaternion.identity);
        }        
    }

    private void DestruirGrupoSlimes()
    {
        // Verificar si el grupo existe antes de intentar destruirlo
        if (gruposSlimes.ContainsKey(grupoID))
        {
            Debug.Log($"Destruyendo grupo {grupoID} con {gruposSlimes[grupoID].Count} slimes.");

            // Crear una copia de la lista para evitar problemas al modificarla durante la iteración
            var slimesDelGrupo = gruposSlimes[grupoID].ToList();

            foreach (var slime in slimesDelGrupo)
            {
                if (slime != null)
                {
                    Destroy(slime.gameObject); // Destruir físicamente el slime
                }
            }

            // Eliminar el grupo del diccionario
            gruposSlimes.Remove(grupoID);
        }
        else
        {
            Debug.LogWarning($"Intento de destruir un grupo inexistente: {grupoID}");
        }
    }
    #endregion

    #region Movimiento y Cambio de Color
    private void MoverHaciaJugador()
    {
        if (!enSalto && navMeshAgent != null && jugador != null)
        {
            float distancia = Vector3.Distance(transform.position, jugador.position);

            // Cambiar al estado Attacking si está dentro del rango de ataque
            if (distancia <= rangoAtaque && !activandoCollider)
            {
                estadoActual = Estado.Attacking;
                StartCoroutine(CambiarColorYActivarCollider());
                return; // Salir para evitar que inicie un salto mientras ataca
            }

            // Moverse solo si está más lejos que el rango de ataque
            if (distancia > rangoAtaque)
            {
                StartCoroutine(SaltarHaciaJugador());
            }
        }
    }

    private IEnumerator SaltarHaciaJugador()
    {
        enSalto = true;

        // Desactivar temporalmente el NavMeshAgent para controlar el salto manualmente
        navMeshAgent.enabled = false;

        Vector3 posicionInicial = transform.position;
        Vector3 direccionHaciaJugador = (jugador.position - transform.position).normalized;
        Vector3 posicionObjetivo = posicionInicial + direccionHaciaJugador * distanciaSalto;

        float tiempo = 0f;

        while (tiempo < duracionSalto)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionSalto;

            // Movimiento parabólico para simular el salto
            Vector3 posicionIntermedia = Vector3.Lerp(posicionInicial, posicionObjetivo, t);
            posicionIntermedia.y += Mathf.Sin(t * Mathf.PI) * alturaSalto;

            transform.position = posicionIntermedia;
            yield return null;
        }

        // Asegurarse de que el slime termine exactamente en la posición objetivo
        transform.position = posicionObjetivo;

        // Reactivar el NavMeshAgent
        navMeshAgent.enabled = true;

        // Esperar 0.5 segundos antes de permitir otro movimiento
        yield return new WaitForSeconds(0.5f);

        enSalto = false;
    }

    private IEnumerator CambiarColorYActivarCollider()
    {
        activandoCollider = true;
        float tiempo = 0f;

        while (tiempo < tiempoCambioColor)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / tiempoCambioColor;
            rendererEsqueleto.material.color = Color.Lerp(colorInicial, colorFinal, t);
            yield return null;
        }

        ActivarCollider();
        InstanciarAtaque(); // Instanciar el ataque después de activar el collider

        // Esperar antes de volver a Chasing y restaurar el color original
        yield return new WaitForSeconds(tiempoEsperaAtaque);

        // Reanudar el movimiento del NavMeshAgent después del ataque
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false;
        }

        rendererEsqueleto.material.color = colorInicial;
        estadoActual = Estado.Chasing;
        activandoCollider = false;
    }

    private void ActivarCollider()
    {
        // Detener el movimiento del NavMeshAgent al atacar
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    private void InstanciarAtaque()
    {
        if (prefabAtaque != null && puntoAtaque != null)
        {
            Instantiate(prefabAtaque, puntoAtaque.position, puntoAtaque.rotation);
        }
    }
    #endregion
}
