using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
/* {
    List<string> tarotCards = new List<string>() { "Strength", "HighPriestess", "Devil"};
    public string selectedCard;

        public void SelectRandomCard()
    {
        if(tarotCards.Count > 0)
        {
            // Si "Devil" está en la lista y también lo están las otras dos cartas, excluye "Devil" de la selección
            if (tarotCards.Contains("Devil") && (tarotCards.Contains("Strength") || tarotCards.Contains("HighPriestess")))
            {
                List<string> selectableCards = new List<string>(tarotCards);
                selectableCards.Remove("Devil");

                int randomIndex = Random.Range(0, selectableCards.Count);
                selectedCard = selectableCards[randomIndex];
            }
            else
            {
                int randomIndex = Random.Range(0, tarotCards.Count);
                selectedCard = tarotCards[randomIndex];
            }

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
   //public void Update()
   // {
   //     if (Input.GetKeyDown(KeyCode.Q))
   //     {
   //         SelectRandomCard();
   //     }

   //     if (Input.GetKeyDown(KeyCode.W))
   //     {
   //         LoadLevel();
   //     }

   //     if (Input.GetKeyDown(KeyCode.E))
   //     {
   //         OnLevelCompleted();
   //     }

   //     if (Input.GetKeyDown(KeyCode.R))
   //     {
   //         OnLevelFailed();
   //     }

   //     if (Input.GetKeyDown(KeyCode.T))
   //     {
   //         Debug.Log("Quedan " + tarotCards.Count + " cartas: " + string.Join(", ", tarotCards));
   //     }
   // }
    #endregion DEBUG
}
 */

/* {
    List<string> tarotCards = new List<string>() { "Strength", "HighPriestess", "Devil"};
    public string selectedCard;

    public void SelectCard()
    {
        if(tarotCards.Count > 0)
        {
            selectedCard = tarotCards[0];
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
        Debug.Log("Carta: " + SceneManager.GetActiveScene().name + " eliminada. Cargando Hub.");
        tarotCards.Remove(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Hub");
    }

    public void OnLevelFailed()
    {
        Debug.Log("Perdiste. Cargando Hub.");
        SceneManager.LoadScene("Hub");
    }

      public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectCard();
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
} */

{
    List<string> tarotCards = new List<string>() { "Strength", "HighPriestess", "Devil"};
    public string selectedCard;

    public int actualLevel;

        public void SelectCard()
    {
        if(tarotCards.Count > 0)
        {/* 
            // Si "Devil" está en la lista y también lo están las otras dos cartas, excluye "Devil" de la selección
            if (tarotCards.Contains("Devil") && (tarotCards.Contains("Strength") || tarotCards.Contains("HighPriestess")))
            {
                List<string> selectableCards = new List<string>(tarotCards);
                selectableCards.Remove("Devil");

                int randomIndex = Random.Range(0, selectableCards.Count);
                selectedCard = selectableCards[randomIndex];
            }
            else
            {
                int randomIndex = Random.Range(0, tarotCards.Count);
                selectedCard = tarotCards[randomIndex];
            }

            Debug.Log("Se ha escogido la carta: " + selectedCard); */
            selectedCard = tarotCards[actualLevel];
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
            Debug.Log(+actualLevel + "antes cargar");
            Debug.Log("Cargando nivel: " + selectedCard);
            Debug.Log(+actualLevel + "despues cargar");
            SceneManager.LoadScene(selectedCard);
        }

        else
        {
            Debug.Log("No se ha seleccionado ninguna carta");
        }
    }

    public void OnLevelCompleted()
    {
        //tarotCards.Remove(SceneManager.GetActiveScene().name);
        //Debug.Log("Carta: " + SceneManager.GetActiveScene().name + " eliminada. Cargando Hub.");
        actualLevel++;
        Debug.Log(+actualLevel+"completed");
        SceneManager.LoadScene("Hub");

        if (actualLevel == 1)
        SceneManager.LoadScene("HighPriestess");

        if (actualLevel == 2)
        SceneManager.LoadScene("Devil");

        if (actualLevel == 3)
        SceneManager.LoadScene("Fin");
    }

    public void OnLevelFailed()
    {
        Debug.Log("Perdiste. Cargando Hub.");
        SceneManager.LoadScene("Hub");
    }

    #region DEBUG //    ***** DEBUG ***** 
   //public void Update()
   // {
   //     if (Input.GetKeyDown(KeyCode.Q))
   //     {
   //         SelectCard();
   //     }

   //     if (Input.GetKeyDown(KeyCode.W))
   //     {
   //         LoadLevel();
   //     }

   //     if (Input.GetKeyDown(KeyCode.E))
   //     {
   //         OnLevelCompleted();
   //     }

   //     if (Input.GetKeyDown(KeyCode.R))
   //     {
   //         OnLevelFailed();
   //     }

   //     if (Input.GetKeyDown(KeyCode.T))
   //     {
   //         //Debug.Log("Quedan " + tarotCards.Count + " cartas: " + string.Join(", ", tarotCards));
   //         Debug.Log(+actualLevel + "debug");
   //     }
   // }
    #endregion DEBUG
}