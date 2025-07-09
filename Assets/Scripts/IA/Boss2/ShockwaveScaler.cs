using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveScaler : MonoBehaviour
{
    public float scaleSpeed = 10f;
    public float maxScale = 6f;
    public float damage = 50f;

    private bool hasHitPlayer = false;

    void Update()
    {
        if (transform.localScale.x < maxScale)
        {
            float newScale = transform.localScale.x + scaleSpeed * Time.deltaTime;
            transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHitPlayer && other.CompareTag("Player"))
        {
            hasHitPlayer = true;
            Debug.Log("Jugador golpeado por la onda expansiva");

            Life vida = other.GetComponent<Life>();
            if (vida != null)
            {
                vida.ModifyTime(-damage);
            }
        }
    }
}