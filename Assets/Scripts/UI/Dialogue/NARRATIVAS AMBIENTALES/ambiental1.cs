using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambiental1 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Mmmm, veo que aqui hay un montón de carretas de carbón, esto debe ser lo que le da energia a las calderas");           
        }
    }
}
