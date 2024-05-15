using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogueTrigger2 : MonoBehaviour
{
    [Header("Icono")]
    [SerializeField] private GameObject icono;
  

    [Header("InkJSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerInRange;

    private void Awake()
    {
        playerInRange = true;
        icono.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !dialogueManager2.GetInstance().dialogueIsPlaying)
        {
            icono.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                dialogueManager1.GetInstance().EnterDialogueMode(inkJSON);
            }
        }
        else
        {
            icono.SetActive(false);
        }   

    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            playerInRange= true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false;
            
        }
    }
}
