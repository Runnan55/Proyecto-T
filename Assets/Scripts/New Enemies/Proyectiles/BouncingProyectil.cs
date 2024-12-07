using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingProyectil : MonoBehaviour
{
    public int maxBounces = 3; // Número máximo de rebotes
    private int bounceCount = 0; // Contador de rebotes
    public float speed = 5f; // Velocidad del objeto
    private Vector3 velocity; // Dirección del movimiento
    public float angleVariation = 15f; // Variación máxima en el ángulo del rebote

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

                // Variar el ángulo del rebote
                normal = AddRandomAngleToNormal(normal);

                // Reflejar la dirección de la velocidad usando la normal ajustada
                velocity = Vector3.Reflect(velocity, normal);

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

    private Vector3 AddRandomAngleToNormal(Vector3 normal)
    {
        // Generar un pequeño ángulo aleatorio en los ejes X y Z
        float randomAngleX = Random.Range(-angleVariation, angleVariation);
        float randomAngleZ = Random.Range(-angleVariation, angleVariation);

        // Crear un vector de variación
        Vector3 variation = new Vector3(randomAngleX, 0, randomAngleZ);

        // Ajustar la normal con la variación
        Vector3 modifiedNormal = (normal + variation).normalized;
        return modifiedNormal;
    }
}