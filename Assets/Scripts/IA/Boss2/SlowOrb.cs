using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowOrb : MonoBehaviour
{
    public float detectionRadius = 8f;
    public float speed = 2f;
    public float lifespan = 10f;

    public float slowFactor = 0.3f;
    public float slowDuration = 1.5f;

    private Transform player;
    private bool activated = false;
    private float timer = 0f;
    private bool effectTriggered = false;

    private static bool playerSlowed = false;
    private static float originalSpeed;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (effectTriggered) return;

        timer += Time.deltaTime;

        if (timer >= lifespan)
        {
            Destroy(gameObject);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (!activated && distance <= detectionRadius)
        {
            activated = true;
        }

        if (activated)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (effectTriggered) return;

        if (other.CompareTag("Player") && !playerSlowed)
        {
            effectTriggered = true;
            playerSlowed = true;

            originalSpeed = MovimientoJugador.speed;
            MovimientoJugador.speed = originalSpeed * slowFactor;

            Debug.Log("Jugador ralentizado. Engranaje oculto.");

            // Ocultar solo el MeshRenderer del hijo "Engranaje"
            Transform engranaje = transform.Find("Engranaje");
            if (engranaje != null)
            {
                MeshRenderer mesh = engranaje.GetComponent<MeshRenderer>();
                if (mesh != null) mesh.enabled = false;
            }

            // Desactivar colisión del orbe
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            StartCoroutine(RestoreAfterDelay());
        }
    }

    private IEnumerator RestoreAfterDelay()
    {
        yield return new WaitForSeconds(slowDuration);

        MovimientoJugador.speed = originalSpeed;
        playerSlowed = false;

        Debug.Log("Velocidad restaurada. Orbe destruido.");
        Destroy(gameObject);
    }

    public void BreakOrb()
    {
        Destroy(gameObject);
    }
}