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

    private IEnumerator Start()
    {
        Destroy(gameObject, lifetime);
        yield return new WaitForSeconds(1f);
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