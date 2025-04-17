using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambiental5 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "ULTIMA HORA, Se han detenido diversas celulas rebeldes en la ciudad superior, el Puente ha decidido sentenciar a muerte a todos los detenidos");
        }
    }
}
