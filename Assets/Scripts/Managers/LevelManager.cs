using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    List<string> tarotCards = new List<string>() { "Strength", "Empress"};
    string selectedCard;

    void SelectRandomCard()
    {
        if(tarotCards.Count > 0)
        {
            int randomIndex = Random.Range(0, tarotCards.Count);
            selectedCard = tarotCards[randomIndex];
            Debug.Log("Se ha escogido la carta: " + selectedCard);
        }

        else
        {
            Debug.Log("No quedan cartas");
        }
    }

    public void LoadLevel()
    {
        if (!string.IsNullOrEmpty(selectedCard))
        {
            Debug.Log("Cargando nivel: " + selectedCard);
            SceneManager.LoadScene(selectedCard);
        }

        else
        {
            Debug.Log("No se ha seleccionado ninguna carta");
        }
    }

    public void OnLevelCompleted()
    {
        tarotCards.Remove(SceneManager.GetActiveScene().name);
        Debug.Log("Carta: " + SceneManager.GetActiveScene().name + " eliminada. Cargando Hub.");
        SceneManager.LoadScene("Hub");
    }

    public void OnLevelFailed()
    {
        Debug.Log("Perdiste. Cargando Hub.");
        SceneManager.LoadScene("Hub");
    }

    #region DEBUG //    ***** DEBUG ***** 
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectRandomCard();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            LoadLevel();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnLevelCompleted();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnLevelFailed();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Quedan " + tarotCards.Count + " cartas: " + string.Join(", ", tarotCards));
        }
    }
    #endregion DEBUG
}

