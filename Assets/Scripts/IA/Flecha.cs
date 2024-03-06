using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flecha : Enemy
{
    public float speed = 10f;
    public float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        damage = 10;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AttackPlayer();
            Debug.Log("pummm");
            Destroy(gameObject);
        }
    }
}