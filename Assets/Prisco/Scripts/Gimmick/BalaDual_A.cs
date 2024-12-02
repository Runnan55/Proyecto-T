using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaDual_A : MonoBehaviour
{
   public float speed = 10f; // Velocidad de la bala
    private const float serpentineFrequency = 2f; // Frecuencia del movimiento en "S"
    private const float serpentineMagnitude = 0.5f; // Magnitud del movimiento en "S"

    private Vector3 direction; // Dirección de la bala
    private float startTime; // Tiempo de inicio para el movimiento en "S"

    // Start is called before the first frame update
    void Start()
    {
        direction = transform.forward;
        startTime = Time.time; // Guardar el tiempo de inicio
    }

    // Update is called once per frame
    void Update()
    {
        // Calcular el tiempo transcurrido desde la instanciación
        float elapsedTime = Time.time - startTime;

        // Movimiento en "S"
        Vector3 serpentine = transform.right * Mathf.Sin(elapsedTime * serpentineFrequency) * serpentineMagnitude;
        Vector3 moveDirection = direction + serpentine;

        // Aplicar el movimiento combinado
        transform.position += moveDirection * speed * Time.deltaTime;

        // Ajustar la rotación para que la bala mire en la dirección del movimiento
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
}
