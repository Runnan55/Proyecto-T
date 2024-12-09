using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingProyectil : MonoBehaviour
{
    public int maxBounces = 3; // Número máximo de rebotes
    private int bounceCount = 0; // Contador de rebotes
    public float speed = 5f; // Velocidad del objeto
    private Vector3 velocity; // Dirección del movimiento

    void Start()
    {
        // Dirección inicial del objeto
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

                // Agregar una ligera desviación al ángulo de la normal para obtener un rebote con ángulo
                Vector3 reflectDir = Vector3.Reflect(velocity, normal);

                // Aplicar un pequeño ángulo aleatorio para desviar ligeramente el rebote
                float angleVariance = Random.Range(-15f, 15f);  // Desviación entre -15 y 15 grados
                reflectDir = Quaternion.Euler(0, angleVariance, 0) * reflectDir;

                // Asegurarnos de que la dirección del movimiento sea normalizada y multiplicada por la velocidad
                velocity = reflectDir.normalized * speed;

                bounceCount++; // Incrementar el contador de rebotes
            }
            else
            {
                Destroy(gameObject); // Destruir el objeto tras el cuarto rebote
            }
        }
        else if (other.CompareTag("Player")) // Colisión con el jugador
        {
            // Aquí puedes agregar lógica adicional, por ejemplo, restar vida al jugador
            Destroy(gameObject); // Destruir el objeto al colisionar con el jugador
        }
    }
}