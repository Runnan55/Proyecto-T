using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour
{
    public Transform target; // El jugador u objeto a seguir
    public float smoothSpeed = 0.125f; // La velocidad de suavizado
    private Vector3 offset; // La distancia inicial entre la cámara y el jugador

    private void Start()
    {
        StartCoroutine(FindPlayerWithDelay(0.5f));
    }

    private IEnumerator FindPlayerWithDelay(float delay)
    {
        // Espera el tiempo especificado antes de ejecutar la búsqueda del jugador
        yield return new WaitForSeconds(delay);

        // Busca al jugador por su tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Si se encuentra el jugador, asignar su transform como el objetivo (target)
        if (player != null)
        {
            target = player.transform;
            offset = transform.position - target.position;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el tag 'Player'.");
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}
