using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambiental7 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CreateDialogue("ABUELO SEXY", "Piedras azules... Asi que esto es lo que estan extrayendo con mis poderes");
        }
    }
}
