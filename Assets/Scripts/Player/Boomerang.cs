using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float velocidadInicial = 5f;
    public float velocidadRegreso = 10f;
    public float tiempoDeEspera = 1f;

    private Vector3 objetivo;
    private bool haciaElObjetivo = true;
    private bool pp = false;
    private bool pp2 = false;
    private bool pp3 = false;
    private float tiempoDeInstanciacion;
    public string etiquetaJugador = "Player"; // Etiqueta del jugador

    //
    public float damage = 5f;
    private float timeSinceLastDamage = 0f;

    void Start()
    {
        // Guardar el tiempo de instanciación
        tiempoDeInstanciacion = Time.time;
    }

    void Update()
    {
        // Si el boomerang está yendo hacia el objetivo
        if (haciaElObjetivo)
        {
            // Si ha pasado el tiempo de espera desde la instanciación, cambia la dirección al regreso
            if (Time.time - tiempoDeInstanciacion > tiempoDeEspera)
            {
                haciaElObjetivo = false;
                // Detiene el boomerang
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                // Guarda la posición actual del boomerang como objetivo de regreso
                GameObject jugador = GameObject.FindGameObjectWithTag(etiquetaJugador);
                objetivo = jugador.transform.position;
                pp2 = true;
                // Inicia el movimiento de regreso hacia el objetivo guardado
            }

        }
                if (Time.time - tiempoDeInstanciacion  > tiempoDeEspera+1f && !pp && pp2)
                {
                    Debug.Log("vuelve");
                    pp = true;
                GameObject jugador = GameObject.FindGameObjectWithTag(etiquetaJugador);
                objetivo = jugador.transform.position;
                    GetComponent<Rigidbody>().velocity = (objetivo- transform.position).normalized * velocidadRegreso;
                    pp3 = true;
                }
                if (Time.time - tiempoDeInstanciacion  > tiempoDeEspera+2f &&pp3)
                {
                    Destroy(gameObject);
                }


        timeSinceLastDamage += Time.deltaTime;
    }

        void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (timeSinceLastDamage >= 0.25f)
            {
                other.gameObject.GetComponent<Enemy>().ReceiveDamage(damage);
                timeSinceLastDamage = 0f;
            }
        }
    }

    // Método para iniciar el movimiento del boomerang con una dirección dada
    public void IniciarMovimiento(Vector3 direccion)
    {
        // Inicia el movimiento hacia la dirección especificada
        GetComponent<Rigidbody>().velocity = direccion * velocidadInicial;
    }
}