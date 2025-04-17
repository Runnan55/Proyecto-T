using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambiental3 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Estas rocas generan energia temporal, cuanto mas me adentro más la siento, Watt debe estar cerca");
        }
    }
}
