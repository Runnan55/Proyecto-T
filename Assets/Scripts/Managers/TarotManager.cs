using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TarotManager : MonoBehaviour
{
    public LevelManager levelManager;
    public GameObject tarotCardUI;
    public TextMeshProUGUI arcanaNameText;

    public GameObject centralObject; // El objeto alrededor del cual los otros girarán
    public GameObject[] rotatingObjects; // Los objetos que girarán
    public float moveDuration = 5f; // La duración del movimiento en segundos

    void OnTriggerEnter(Collider other)
    {
        StartTarot();
    }

    void StartTarot()
    {
        StartCoroutine(PlayAnimationAndLoadLevel());
    }

    public IEnumerator PlayAnimationAndLoadLevel()
    {
        levelManager.SelectRandomCard();

        // Activa las cartas antes de iniciar la animación
        foreach (GameObject obj in rotatingObjects)
        {
            obj.SetActive(true);
        }
        
        // Inicia el movimiento
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            foreach (GameObject obj in rotatingObjects)
            {
                // Calcula la nueva posición del objeto
                Vector3 newPosition = Vector3.Lerp(obj.transform.position, centralObject.transform.position, elapsedTime / moveDuration);

                // Actualiza la posición del objeto
                obj.transform.position = newPosition;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Muestra el nombre de la arcana seleccionada
        arcanaNameText.gameObject.SetActive(true);
        arcanaNameText.text = levelManager.selectedCard;
        tarotCardUI.SetActive(true);

        // Espera 3 segundos
        yield return new WaitForSeconds(3f);

        levelManager.LoadLevel();
    }
}