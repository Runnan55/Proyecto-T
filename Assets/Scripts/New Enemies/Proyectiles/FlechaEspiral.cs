using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlechaEspiral: MonoBehaviour
{
    public float speed = 5f; // Velocidad constante de la flecha
    public float initialRadius = 0.1f; // Radio inicial de la espiral
    public float radiusGrowthRate = 1f; // Tasa de crecimiento del radio
    public float lifetime = 30f; // Tiempo de vida de la flecha
    public float damage = 5f; // Da�o infligido al jugador

    private Life playerLife;
    private float angle = 0f; // �ngulo acumulado para calcular la espiral
    private Vector3 startPosition; // Punto de inicio de la flecha

    private void Start()
    {
        // Guardar la posici�n inicial
        startPosition = transform.position;

        // Destruir el objeto despu�s de su tiempo de vida
        Destroy(gameObject, lifetime);

        // Referencia a la vida del jugador
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>();
    }

    private void Update()
    {
        // Calcular el radio actual con un crecimiento lineal
        float currentRadius = initialRadius + radiusGrowthRate * angle;

        // Mantener velocidad constante ajustando el incremento del �ngulo
        float angularSpeed = speed / currentRadius; // Velocidad angular adaptada al radio actual
        angle += angularSpeed * Time.deltaTime;

        // Calcular la nueva posici�n en espiral en el plano XZ
        float xOffset = Mathf.Cos(angle) * currentRadius;
        float zOffset = Mathf.Sin(angle) * currentRadius;

        // Actualizar la posici�n manteniendo la altura constante (Y fija)
        transform.position = new Vector3(startPosition.x + xOffset, startPosition.y, startPosition.z + zOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reducir la vida del jugador
            playerLife.ModifyTime(-damage);
            MovimientoJugador.isInDodgeArea = false;

            // Destruir la flecha
            Destroy(gameObject);
        }
        else if (other.CompareTag("Walls"))
        {
            // Destruir la flecha al colisionar con paredes
            Destroy(gameObject);
        }
    }
}