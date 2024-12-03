using UnityEngine;

public class Zone : MonoBehaviour
{
    public int zoneID; // Identificador �nico de la zona

    // Al entrar el jugador en la zona, se le notifica al Boss
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Aseg�rate de que el jugador tiene el tag "Player"
        {
            BossMovement boss = FindObjectOfType<BossMovement>(); // Busca al Boss en la escena
            if (boss != null)
            {
                boss.OnPlayerEnterZone(zoneID); // Informa al Boss sobre la zona
            }
        }
    }
}