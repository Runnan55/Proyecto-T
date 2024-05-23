using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vasijas : MonoBehaviour
{
    public GameObject dropItem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(dropItem, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }

}
