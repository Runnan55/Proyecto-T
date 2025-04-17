using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambiental4 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Peligro de lesiones, precaucion al manupular estos compuestos");
        }
    }
}
