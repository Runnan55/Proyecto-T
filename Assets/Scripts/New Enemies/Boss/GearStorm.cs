using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearStorm : MonoBehaviour
{
    [Header("Configuraci�n de la Tormenta")]
    public GameObject gearMarkerPrefab; // Prefab para el marcador de previsualizaci�n
    public GameObject gearProjectilePrefab; // Prefab del engranaje
    public int cantidadEngranajes = 10; // N�mero de engranajes a lanzar
    public float radioTormenta = 20f; // Radio m�ximo donde caen los engranajes
    public float tiempoMarcador = 1.5f; // Tiempo que duran los marcadores antes de desaparecer

  
    public void IniciarTormenta(Vector3 centro)
    {
        Debug.Log("Iniciando Tormenta de Engranajes");
        for (int i = 0; i < cantidadEngranajes; i++)
        {
            // Calcular una posici�n aleatoria dentro del radio
            Vector3 posicionAleatoria = CalcularPosicionAleatoria(centro, radioTormenta);

            // Instanciar marcador temporal para la previsualizaci�n
            GameObject marcador = Instantiate(gearMarkerPrefab, posicionAleatoria, Quaternion.identity);
            Destroy(marcador, tiempoMarcador); // Destruir marcador tras el tiempo establecido

            // Instanciar el engranaje con un retraso para sincronizarlo con la desaparici�n del marcador
            StartCoroutine(InstanciarEngranajeConRetraso(posicionAleatoria, tiempoMarcador));
        }
    }

    private Vector3 CalcularPosicionAleatoria(Vector3 centro, float radio)
    {
        // Generar una posici�n aleatoria dentro de un c�rculo
        Vector2 puntoAleatorio = Random.insideUnitCircle * radio;
        return new Vector3(centro.x + puntoAleatorio.x, centro.y, centro.z + puntoAleatorio.y);
    }

    private IEnumerator InstanciarEngranajeConRetraso(Vector3 posicion, float retraso)
    {
        yield return new WaitForSeconds(retraso);

        // Instanciar el engranaje en la posici�n calculada
        Instantiate(gearProjectilePrefab, posicion, Quaternion.identity);
    }
}