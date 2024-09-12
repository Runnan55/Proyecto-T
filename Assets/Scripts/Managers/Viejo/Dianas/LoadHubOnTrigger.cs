using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadHubOnTrigger : MonoBehaviour
{
    // Nombre de la escena a cargar
    public string sceneName = "Hub";

    // Método que se llama cuando otro collider entra en el trigger
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entró en el trigger es el jugador
        if (other.CompareTag("Player"))
        {
            // Carga la escena especificada
            SceneManager.LoadScene(sceneName);
        }
    }
}
