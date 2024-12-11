using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearStorm : MonoBehaviour
{
    [Header("Configuración de la Tormenta")]
    public GameObject gearMarkerPrefab; // Prefab para el marcador de previsualización
    public GameObject gearProjectilePrefab; // Prefab del engranaje
    public int cantidadEngranajes = 10; // Número de engranajes a lanzar
    public float radioTormenta = 20f; // Radio máximo donde caen los engranajes
    public float tiempoMarcador = 1.5f; // Tiempo que duran los marcadores antes de desaparecer

  
    public void IniciarTormenta(Vector3 centro)
    {
        Debug.Log("Iniciando Tormenta de Engranajes");
        for (int i = 0; i < cantidadEngranajes; i++)
        {
            // Calcular una posición aleatoria dentro del radio
            Vector3 posicionAleatoria = CalcularPosicionAleatoria(centro, radioTormenta);

            // Instanciar marcador temporal para la previsualización
            GameObject marcador = Instantiate(gearMarkerPrefab, posicionAleatoria, Quaternion.identity);
            Destroy(marcador, tiempoMarcador); // Destruir marcador tras el tiempo establecido

            // Instanciar el engranaje con un retraso para sincronizarlo con la desaparición del marcador
            StartCoroutine(InstanciarEngranajeConRetraso(posicionAleatoria, tiempoMarcador));
        }
    }

    private Vector3 CalcularPosicionAleatoria(Vector3 centro, float radio)
    {
        // Generar una posición aleatoria dentro de un círculo
        Vector2 puntoAleatorio = Random.insideUnitCircle * radio;
        return new Vector3(centro.x + puntoAleatorio.x, centro.y, centro.z + puntoAleatorio.y);
    }

    private IEnumerator InstanciarEngranajeConRetraso(Vector3 posicion, float retraso)
    {
        yield return new WaitForSeconds(retraso);

        // Instanciar el engranaje en la posición calculada
        Instantiate(gearProjectilePrefab, posicion, Quaternion.identity);
    }
}