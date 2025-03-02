using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballEnemy : MonoBehaviour
{
    public float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime);

    }

    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        Life player = other.gameObject.GetComponent<Life>();
        if (other.gameObject.CompareTag("Player"))
        {
            player.ModifyTime(-50f);

            Debug.Log("Golpe a un enemigo");
            Destroy(gameObject); // Destruye el objeto que golpea
        }
        else if (other.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject); // Destruye el objeto que golpea
        }



    }
}
