using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public List<GameObject> doors; // Lista de puertas
    public int[] enemiesPerRoom; // Número de enemigos por sala
    private int currentRoom = 0; // Sala actual
    private int defeatedEnemies = 0; // Contador de enemigos derrotados
    public float doorSpeed = 1f; // Velocidad a la que las puertas bajan

    // Start is called before the first frame update
    void Start()
    {
        if (doors.Count > 0)
        {
            // Asegurarse de que todas las puertas estén cerradas al inicio
            foreach (GameObject door in doors)
            {
                door.SetActive(true);
            }
        }
    }

    // Este método se llama cuando un enemigo es derrotado
    public void EnemyDefeated()
    {
        Debug.Log("Enemy defeated en doormanager");

        defeatedEnemies++; // Incrementar el contador de enemigos derrotados

        Debug.Log("Defeated enemies: " + defeatedEnemies); // Agregar un registro de depuración para verificar el número de enemigos derrotados

        // Verificar si todos los enemigos en la sala actual han sido derrotados
        if (defeatedEnemies >= enemiesPerRoom[currentRoom])
        {
            // Si hay más salas, abrir la puerta a la siguiente sala
            if (currentRoom < doors.Count)
            {
                Debug.Log("Opening door"); // Agregar un registro de depuración para verificar que la puerta se está abriendo
                StartCoroutine(OpenDoor(doors[currentRoom]));
            }

            // Pasar a la siguiente sala
            currentRoom++;

            // Resetear el contador de enemigos derrotados
            defeatedEnemies = 0;
        }
    }

    // Coroutine para abrir la puerta
    IEnumerator OpenDoor(GameObject door)
    {      
        Debug.Log("abriendo puerta");
        float timer = 0;

        while (timer < 5)
        {
            door.transform.Translate(0, -doorSpeed * Time.deltaTime, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        door.SetActive(false);
    }
}
