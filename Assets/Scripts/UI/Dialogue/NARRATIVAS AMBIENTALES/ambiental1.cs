using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambiental1 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Mmmm, veo que aqui hay un mont�n de carretas de carb�n, esto debe ser lo que le da energia a las calderas");           
        }
    }
}
