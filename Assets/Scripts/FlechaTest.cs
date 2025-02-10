using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlechaTest : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 30f;
    public float damage = 5f;

    private Life playerLife;
    private MovimientoJugador movimientoJugador;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player
        movimientoJugador = GameObject.FindGameObjectWithTag("Player").GetComponent<MovimientoJugador>(); // referencia MovimientoJugador
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * MovimientoJugador.bulletTimeScale * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerLife.ModifyTime(-damage);
            Destroy(gameObject);
        }

        if (other.CompareTag("BTCollider"))
        {
            movimientoJugador.CountBTProjectiles();
        }
        else if (other.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }

    public void OnDisable()
    {
        movimientoJugador.CountBTProjectiles();
    }

    public void OnDestroy()
    {
        movimientoJugador.CountBTProjectiles();
    }
}