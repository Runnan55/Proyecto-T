using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogo2 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Hay demasiadas trampas, me estan ralentizando mucho, y encima no tengo el total de mis poderes");
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "No exageraban cuando decian que era un genio tecnológico, las trampas son dificiles de eludir");
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Y encima alimentadas de energia temporal, amplifican su rango en un 200%");


            Destroy(gameObject, 1f);
        }
    }
}
