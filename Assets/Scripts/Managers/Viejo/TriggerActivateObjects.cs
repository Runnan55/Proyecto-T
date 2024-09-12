using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActivateObjects : MonoBehaviour
{
    public GameObject objectToActivate;
    public float delayInSeconds = 120; // 120 segundos = 2 minutos

    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si el objeto que entra al trigger es el jugador
        if (other.CompareTag("Player"))
        {
            // Activa los GameObjects
            ActivateObjects(true);

            // Planifica la desactivación de los GameObjects
            Invoke(nameof(DeactivateObjects), delayInSeconds);
        }
    }

    private void ActivateObjects(bool isActive)
    {
        if (objectToActivate != null)
            objectToActivate.SetActive(isActive);

    }

    private void DeactivateObjects()
    {
        ActivateObjects(false);
    }
}