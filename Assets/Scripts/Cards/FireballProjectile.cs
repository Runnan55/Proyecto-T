using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Golpe a un enemigo");
            Destroy(gameObject); // Destruye el objeto que golpea
        }
        else if (other.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject); // Destruye el objeto que golpea
        }
    }
}