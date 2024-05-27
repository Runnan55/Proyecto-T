using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInventory : MonoBehaviour
{
    private ICard[] slots = new ICard[3];       //  SLOT IZQUIERDO = 1      SLOT CENTRAL = 0     SLOT DERECHO = 2

    private Dictionary<string, ICard> cardDictionary = new Dictionary<string, ICard>();    

    public Image slot1Image;
    public Image slot2Image;
    public Image slot3Image;
    public Sprite defaultImage;

    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public TextMeshProUGUI slot3Text;

    //private static int cardCounter = 0;

    public bool AddCard(string id, ICard card)
    {
        if (!cardDictionary.ContainsKey(id))
        {
            cardDictionary.Add(id, card);
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                {
                    slots[i] = card;
                    return true;
                }
            }
        }
        return false;
    }

    public void RotateLeft()
    {
        ICard temp = slots[0];
        slots[0] = slots[1];
        slots[1] = slots[2];
        slots[2] = temp;
    }

    public void RotateRight()
    {
        ICard temp = slots[2];
        slots[2] = slots[1];
        slots[1] = slots[0];
        slots[0] = temp;
    }

    public void UseCard()
    {
        if (slots[0] != null)
        {
            //***DEBUG***
            Debug.Log("Carta usada: " + slots[0].Name);
            slots[0].Effect();
            slots[0] = null;
        }
        else
        {
            Debug.Log("No hay carta para usar");
        }
    }

    public void Update()
    {
        slot1Image.sprite = slots[0]?.Image ?? defaultImage;
        slot2Image.sprite = slots[1]?.Image ?? defaultImage;
        slot3Image.sprite = slots[2]?.Image ?? defaultImage;

        slot1Text.text = slots[0]?.Name ?? "Empty";
        slot2Text.text = slots[1]?.Name ?? "Empty";
        slot3Text.text = slots[2]?.Name ?? "Empty";
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateLeft();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateRight();
        }
        
        //***DEBUG***

        if (Input.GetKeyDown(KeyCode.F))
        {
            UseCard();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject cardObject = new GameObject("TeleportCard");
            //C1Teleport teleportCard = cardObject.AddComponent<C1Teleport>();
           // AddCard("card" + cardCounter++, //teleportCard);
            //Destroy(cardObject); 
        }
    }
}