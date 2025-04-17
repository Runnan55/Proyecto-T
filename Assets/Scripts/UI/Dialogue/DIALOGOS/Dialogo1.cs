using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogo1 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "No solo son los guardias, aqui hay mucha mas seguridad de lo que me esperaba");
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Aún así, detecto muchas mas energia temporal que antes, me estoy acercando");
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Aunque debo tener cuidado con las trampas...");


            Destroy(gameObject, 1f);
        }
    }
}
