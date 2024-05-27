using UnityEngine;

public class ClearInventoryAndDestroyCards : MonoBehaviour
{
    public Collider colliderToActivate; // Asigna el collider que quieres activar
    public string playerTag = "Player"; // Tag del jugador que debe entrar en el trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            ClearInventoryAndDestroy();
        }
    }

    public void ClearInventoryAndDestroy()
    {
        // Activar el collider
        if (colliderToActivate != null)
        {
            colliderToActivate.enabled = true;
        }

        // Destruir todos los objetos con tag "card"
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in cards)
        {
            Destroy(card);
        }

        // Llamar al método ClearInventory del script InventarioPlayer
        if (InventarioPlayer.Instance != null)
        {
            InventarioPlayer.Instance.ClearInventory();
            Debug.Log("InventarioPlayer cleared");
        }
    }
}
