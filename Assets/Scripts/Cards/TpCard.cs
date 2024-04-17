using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpCard : BaseCard
{
    // Start is called before the first frame update
    public override void Activate()
    {
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    CharacterController controller = player.GetComponent<CharacterController>();

    // Desactivar el CharacterController
    if (controller != null) controller.enabled = false;

    // Lógica para cambiar la posición aquí


        // Encuentra el jugador en la escena usando el tag
        GameObject playerParent = GameObject.FindGameObjectWithTag("Player");
        // Intenta encontrar el hijo espec�fico dentro del jugador
        Transform playerGroundTransform = playerParent?.transform.Find("PlayerGround");

        GameObject card = GameObject.FindGameObjectWithTag("Tp");

        // Verifica que el jugador y la carta existan en la escena
        if (playerParent != null && playerGroundTransform != null && card != null)
        {
            // Calcula la diferencia de posici�n entre el jugador y el PlayerGround
            Vector3 offset = playerParent.transform.position - playerGroundTransform.position;

            // Almacena temporalmente la posici�n de la carta
            Vector3 cardPosition = card.transform.position;

            // Cambia la posici�n del jugador a la posici�n de la carta, ajustada por el offset
            playerParent.transform.position = cardPosition + offset;

            // Opcionalmente, puedes mover la carta a la posici�n original del PlayerGround o realizar otras acciones
            card.transform.position = playerGroundTransform.position; // Esto mover�a la carta a donde estaba originalmente el PlayerGround
            Debug.Log("TpCard");
                // Reactivar el CharacterController
    if (controller != null) controller.enabled = true;
        }
        else
        {
            // Si no se encuentra el jugador, el hijo espec�fico del jugador o la carta, muestra un mensaje de error
            Debug.LogError("Player, PlayerGround, or Card not found in the scene.");
            Debug.Log("Player: " + playerParent);
            Debug.Log("PlayerGround: " + playerGroundTransform);
            Debug.Log("Card: " + card);
                // Reactivar el CharacterController
    if (controller != null) controller.enabled = true;
        }
        Destroy(this.gameObject);
    }
    }

