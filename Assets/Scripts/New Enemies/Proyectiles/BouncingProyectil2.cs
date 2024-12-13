using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingProyectil2 : MonoBehaviour
{
    public int maxBounces = 3; // N�mero m�ximo de rebotes
    private int bounceCount = 0; // Contador de rebotes
    public float speed = 5f; // Velocidad del objeto
    public float damage = 5f;

    private Vector3 velocity; // Direcci�n del movimiento
    private Life playerLife;

    // M�todo para inicializar la direcci�n del proyectil desde el script que lo instancia
    public void Initialize(Vector3 initialDirection)
    {
        velocity = initialDirection.normalized * speed;
    }
     void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player

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

                // Agregar una ligera desviaci�n al �ngulo de la normal para obtener un rebote con �ngulo
                Vector3 reflectDir = Vector3.Reflect(velocity, normal);

                // Aplicar un peque�o �ngulo aleatorio para desviar ligeramente el rebote
                float angleVariance = Random.Range(-15f, 15f);  // Desviaci�n entre -15 y 15 grados
                reflectDir = Quaternion.Euler(0, angleVariance, 0) * reflectDir;

                // Asegurarnos de que la direcci�n del movimiento sea normalizada y multiplicada por la velocidad
                velocity = reflectDir.normalized * speed;

                bounceCount++; // Incrementar el contador de rebotes
            }
            else
            {
                Destroy(gameObject); // Destruir el objeto tras el cuarto rebote
            }
        }
        else if (other.CompareTag("Player"))
        {
            playerLife.ModifyTime(-damage);
            MovimientoJugador.isInDodgeArea = false;
            Debug.Log("buenas tardes " + MovimientoJugador.isInDodgeArea);

            Destroy(gameObject);
        }
    }
}