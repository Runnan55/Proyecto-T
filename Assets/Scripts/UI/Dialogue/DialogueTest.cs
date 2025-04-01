using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("Kael", "Donde diablos estoy, no veo nada y aparte solo noto calor y mucha pobredumbre");
            DialogueManager.Instance.CreateDialogue("Kael", "Espera, ya se, estoy en la sala de maáquinas, estoy cerca entonces");
            DialogueManager.Instance.CreateDialogue("Lord Watt", "Vaya, vaya, si es el magnifico y sin poderes guardian del tiempo");
            DialogueManager.Instance.CreateDialogue("Kael", "Watt, devuelveme la parte del reloj, si no lo haces creareis un pliegue temporal y destruiremos la realidad tal y como la conocemos");
            DialogueManager.Instance.CreateDialogue("Lord Watt", "Ah viejo amigo, eso no va a ser posible. Gracias a tu maravilloso reloj, hemos aumentado la produccion del mundo un 500%, ¿piensas que te lo vamos a devolver asi tal que asi?");
            DialogueManager.Instance.CreateDialogue("Lord Watt", "Si quieres recuperar tu relojito, adentrate hasta la sala de calderas, te estoy esperando impaciente...");

            Destroy(gameObject, 1f);
        }
    }
}