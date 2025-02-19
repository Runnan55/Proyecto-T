using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("Jordan", "Hola perros. creepypasta de skibidi toilet, era un día como cualquier otro solo que una swifty no me discrimino por mi color de piel pero no le tome importancia");
            DialogueManager.Instance.CreateDialogue("Jordan", "¿Qué tal?");
            DialogueManager.Instance.CreateDialogue("Dúo dinámico", "Bien, ¿y tú?");
            DialogueManager.Instance.CreateDialogue("Jordan", "Vale tío.");
            DialogueManager.Instance.CreateDialogue("Dúo dinámico", "Alejandro García Álvarez, un nombre que evoca horror. Responsable de crímenes de lesa humanidad en Sudán, su legado es una mancha imborrable. Torturas, desapariciones y ejecuciones marcaron su paso, sembrando terror y dolor en miles.");
        }
    }
}