using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flecha : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public float damage = 5f;

    private Life playerLife;


    private void Start()
    {
        Destroy(gameObject, lifetime);
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player

    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerLife.ModifyTime(-damage);

            Debug.Log("pummm");
            Destroy(gameObject);
        }
        else if (other.CompareTag("Walls"))
        {
            Debug.Log("pared"); 
            Destroy(gameObject);
        }
    }
  
}