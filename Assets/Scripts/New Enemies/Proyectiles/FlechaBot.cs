using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlechaBot : MonoBehaviour
{
    public float speed = 10f;      // Velocidad del proyectil
    public float lifetime = 30f;   // Tiempo de vida del proyectil
    public float damage = 5f;      // Daño al jugador

    private Life playerLife;       // Referencia a la vida del jugador
    private Vector3 direction;     // Dirección en la que se mueve el proyectil

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destruir el proyectil después de cierto tiempo
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // Obtener la referencia a la vida del jugador
    }

    // Método para establecer la dirección del proyectil
    public void SetDirection(Vector3 dir)
    {
        dir.y = 0;  // Aseguramos que la dirección en Y sea 0
        direction = dir.normalized; // Normalizamos la dirección
    }

        private void Update()
    {
        // Mover el proyectil en la dirección calculada
        if (direction != Vector3.zero)
        {
            transform.Translate(direction * speed * MovimientoJugador.bulletTimeScale * Time.deltaTime, Space.World);
        }
    }

    // Manejo de las colisiones
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerLife.ModifyTime(-damage);  // Reducir la vida del jugador
            MovimientoJugador.isInDodgeArea = false;  // Desactivar el área de evasión
            Debug.Log("¡El jugador ha sido alcanzado!");

            Destroy(gameObject); // Destruir el proyectil
        }
        else if (other.CompareTag("Walls"))
        {
            Destroy(gameObject); // Destruir el proyectil si colisiona con la pared
        }
    }
}