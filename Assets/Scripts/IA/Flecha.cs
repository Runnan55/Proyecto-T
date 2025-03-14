using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flecha : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 30f;
    public float damage = 5f;

    private Life playerLife;


    private void Start()
    {
        Destroy(gameObject, lifetime);
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player

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
            MovimientoJugador.isInDodgeArea = false;
        //    Debug.Log("buenas tardes " +MovimientoJugador.isInDodgeArea);

            Destroy(gameObject);
        }
        else if (other.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }

    public void OnDisable()
    {
        MovimientoJugador.isInDodgeArea = false;
    }

    public void OnDestroy()
    {
        MovimientoJugador.isInDodgeArea = false;
    }
  
}