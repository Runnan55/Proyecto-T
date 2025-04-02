using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Para el movimiento del enemigo

public class Esqueletos : EnemyLife
{
  #region Variables  
    private Mesh originalMesh;
    private MeshFilter meshFilter; 
    public GameObject areaEffectPrefab;
    public Transform jugador; // Referencia al jugador
    public float velocidad = 3f; // Velocidad de movimiento
    public Color colorInicial = Color.white;
    public Color colorFinal = Color.red;
    public float tiempoCambioColor = 1f;
    private Renderer rendererEsqueleto;
    private bool activandoCollider = false;
    public Transform puntoAtaque; // Objeto vacío donde se instanciará el prefab del ataque
    public GameObject prefabAtaque; // Prefab del ataque

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
        if (jugador == null)
        {
            jugador = GameObject.FindGameObjectWithTag("Player").transform; // Buscar al jugador por tag
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
                    Destroy(gameObject, 0.2f);
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



    #region Movimiento y Cambio de Color
    private void MoverHaciaJugador()
    {
        Vector3 direccion = (jugador.position - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;

        float distancia = Vector3.Distance(transform.position, jugador.position);
        if (distancia <= 2f && !activandoCollider) // Cambiar al estado Attacking al acercarse
        {
            estadoActual = Estado.Attacking;
            StartCoroutine(CambiarColorYActivarCollider());
        }
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
        yield return new WaitForSeconds(tiempoEsperaAtaque); // Esperar antes de volver a Chasing
        estadoActual = Estado.Chasing;
        activandoCollider = false;
    }

    private void ActivarCollider()
    {
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
