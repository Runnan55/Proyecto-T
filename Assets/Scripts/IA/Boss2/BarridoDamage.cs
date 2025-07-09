using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarridoDamage : MonoBehaviour
{
    public float damage = 50f;
    private bool hasDamaged = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasDamaged && other.CompareTag("Player"))
        {
            hasDamaged = true;
            Debug.Log("Jugador golpeado por barrido");

            Life vida = other.GetComponent<Life>();
            if (vida != null)
            {
                vida.ModifyTime(-damage);
            }
        }
    }
}