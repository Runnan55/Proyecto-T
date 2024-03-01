using UnityEngine;

public class CardDrop : MonoBehaviour
{
    public string cardName = "Carta"; // El nombre de la carta que se asignará al inventario

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el collider del jugador tenga el tag "Player"
        {
            if (TryAddCardToInventory())
            {
                Destroy(gameObject); // Destruye el objeto de la carta solo si se ha añadido al inventario
            }
        }
    }

    bool TryAddCardToInventory()
    {
        // Intenta añadir la carta al primer slot "Empty" encontrado en el orden: Slot 2, Slot 3, Slot 1
        string[] slots = InventarioPlayer.Instance.cards;
        
        int[] order = { 1, 2, 0 }; // El orden en que se revisarán los slots: 2, 3, 1
        foreach (int i in order)
        {
            if (slots[i] == "Empty")
            {
                slots[i] = cardName; // Reemplaza "Empty" con el nombre de la carta
                InventarioPlayer.Instance.UpdateInventoryDisplay(); // Actualiza la visualización del inventario
                return true; // Retorna true si se ha añadido la carta exitosamente
            }
        }
        return false; // Retorna false si no se ha encontrado ningún slot "Empty"
    }

    
}
