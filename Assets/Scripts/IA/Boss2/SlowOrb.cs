using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowOrb : MonoBehaviour
{
    public float detectionRadius = 8f;
    public float speed = 2f;
    public float lifespan = 10f;

    private Transform player;
    private bool activated = false;
    private float timer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifespan)
        {
            Debug.Log("Orbe desaparece tras 10s");
            Destroy(gameObject);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (!activated && distance <= detectionRadius)
        {
            activated = true;
            Debug.Log("Orbe ha detectado al jugador");
        }

        if (activated)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
        }
    }

    public void BreakOrb()
    {
        Debug.Log("Orbe destruido por el jugador");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Orbe tocó al jugador");
            Destroy(gameObject);
        }
    }
}