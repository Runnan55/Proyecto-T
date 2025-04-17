using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambiental6 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "GOLDEN GORGON, Un sorbo de riqueza liquida.");
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Caldo de menta de caverna y estofado de ratas carvernosas");
        }
    }
}
