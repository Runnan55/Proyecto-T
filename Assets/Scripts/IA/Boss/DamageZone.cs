using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damageAmount = 10f;
    private Life playerLife;

    void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerLife != null)
            {
                playerLife.ModifyTime(-damageAmount);
            }
        }
    }
}
