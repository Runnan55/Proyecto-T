using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingProyectil : MonoBehaviour
{
    public int maxBounces = 3; // N�mero m�ximo de rebotes
    private int bounceCount = 0; // Contador de rebotes
    public float speed = 5f; // Velocidad del objeto
    private Vector3 velocity; // Direcci�n del movimiento

    void Start()
    {
        // Direcci�n inicial del objeto
        velocity = transform.forward;
        velocity.y = 0; // Mantener movimiento en el plano XZ si es necesario
        velocity = velocity.normalized * speed;
    }

    void Update()
    {
        // Mover el objeto continuamente
        transform.position += velocity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Walls")) // Rebote contra una pared
        {
            if (bounceCount < maxBounces)
            {
                // Calcular el rebote usando la normal de la pared
                Vector3 normal = other.transform.forward;

                // Reflejar la direcci�n de la velocidad usando la normal
                velocity = Vector3.Reflect(velocity, normal);

                bounceCount++; // Incrementar el contador de rebotes
            }
            else
            {
                Destroy(gameObject); // Destruir el objeto tras el cuarto rebote
            }
        }
        else if (other.CompareTag("Player")) // Colisi�n con el jugador
        {
            // Aqu� puedes agregar l�gica adicional, por ejemplo, restar vida al jugador
            Destroy(gameObject); // Destruir el objeto al colisionar con el jugador
        }
    }
}