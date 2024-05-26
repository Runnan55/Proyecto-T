using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public Transform cameraPosition; // La posición de la cámara cuando el jugador esté en esta sala

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Encuentra la cámara principal y llama a ChangeTarget para actualizar la posición objetivo
            CamaraRooms CamaraRooms = Camera.main.GetComponent<CamaraRooms>();
            Camara camara = Camera.main.GetComponent<Camara>();
            if (CamaraRooms != null)
            {
                CamaraRooms.ChangeTarget(cameraPosition);
               

            }
        }
    }
}
