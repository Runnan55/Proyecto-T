using UnityEngine;

public class TriggerActivateObjects : MonoBehaviour
{
    public GameObject objectToActivate1;
    public GameObject objectToActivate2;

    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si el objeto que entra al trigger es el jugador
        if (other.CompareTag("Player"))
        {
            // Activa los GameObjects
            if (objectToActivate1 != null)
                objectToActivate1.SetActive(true);

            if (objectToActivate2 != null)
                objectToActivate2.SetActive(true);
        }
    }
}
