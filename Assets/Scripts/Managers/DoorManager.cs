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

            // Abrir la primera puerta
            StartCoroutine(OpenDoor(doors[0]));
        }
    }

    // Este método se llama cuando un enemigo es derrotado
    public void EnemyDefeated()
    {
        defeatedEnemies++; // Incrementar el contador de enemigos derrotados

        // Verificar si todos los enemigos en la sala actual han sido derrotados
        if (defeatedEnemies >= enemiesPerRoom[currentRoom])
        {
            // Pasar a la siguiente sala
            currentRoom++;

            // Si hay más salas, abrir la puerta a la siguiente sala
            if (currentRoom < doors.Count)
            {
                StartCoroutine(OpenDoor(doors[currentRoom]));
            }

            // Resetear el contador de enemigos derrotados
            defeatedEnemies = 0;
        }
    }

    // Coroutine para abrir la puerta
    IEnumerator OpenDoor(GameObject door)
    {
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
