using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilClones : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f; // Cambiado a 5 segundos
    public float damage = 5f;
    public float spiralSpeed = 5f;
    public float initialSpiralRadius = 2f; // Radio inicial de la espiral
    public float maxSpiralRadius = 5f; // Radio máximo de la espiral
    public float spiralDuration = 5f;

    private float angle = 0f;
    private float spiralRadius;
    private Vector3 initialPosition;

    [SerializeField] private Life playerLife;
    [SerializeField] private MovimientoJugador movimientoJugador;

    void Start()
    {
        Destroy(gameObject, lifetime);
        spiralRadius = initialSpiralRadius; // Inicializar el radio de la espiral
        initialPosition = transform.position; // Guardar la posición inicial
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLife = player.GetComponent<Life>();
            movimientoJugador = player.GetComponent<MovimientoJugador>();
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    void Update()
    {
        angle += spiralSpeed * MovimientoJugador.bulletTimeScale * Time.deltaTime;
        spiralRadius = Mathf.Min(maxSpiralRadius, spiralRadius + (maxSpiralRadius - initialSpiralRadius) / spiralDuration * MovimientoJugador.bulletTimeScale * Time.deltaTime);
        float x = Mathf.Cos(angle) * spiralRadius;
        float z = Mathf.Sin(angle) * spiralRadius;
        Vector3 offset = new Vector3(x, 0, z);
        transform.position = initialPosition + offset; // Mover el proyectil alrededor de la posición inicial
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerLife != null)
            {
                playerLife.ModifyTime(-damage);
            }
            else
            {
                Debug.LogError("playerLife is null!");
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("BTCollider"))
        {
            if (movimientoJugador != null)
            {
                movimientoJugador.CountBTProjectiles();
            }
            else
            {
                Debug.LogError("movimientoJugador is null!");
            }
        }
    }
}
