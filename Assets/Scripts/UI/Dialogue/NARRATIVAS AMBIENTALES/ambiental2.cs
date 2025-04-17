using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambiental2 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Estos trozos de metal, seguro que son partes de nuevas maquinas para generar energia");           
        }
    }
}
