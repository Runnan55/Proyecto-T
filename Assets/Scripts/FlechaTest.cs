using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlechaTest : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 30f;
    public float damage = 5f;

    [SerializeField] private Life playerLife;
    [SerializeField] private MovimientoJugador movimientoJugador;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLife = player.GetComponent<Life>();
            movimientoJugador = player.GetComponent<MovimientoJugador>();
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * MovimientoJugador.bulletTimeScale * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerLife != null)
            {
                playerLife.ModifyTime(-damage);
            }
            else
            {
                Debug.LogError("playerLife is null!");
            }
            Destroy(gameObject);
        }

        else if (other.CompareTag("BTCollider"))
        {
            if (movimientoJugador != null)
            {
                movimientoJugador.CountBTProjectiles();
            }
            else
            {
                Debug.LogError("movimientoJugador is null!");
            }
        }
        
        else if (other.CompareTag("Walls"))
        {
            //Debug.Log("Collision with Walls detected");
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BTCollider"))
        {
            movimientoJugador.CountBTProjectiles();
        }
    }

    public void OnDisable()
    {
        if (movimientoJugador != null)
        {
            movimientoJugador.CountBTProjectiles();
        }
    }

    public void OnDestroy()
    {
        if (movimientoJugador != null)
        {
            movimientoJugador.CountBTProjectiles();
        }
    }
}