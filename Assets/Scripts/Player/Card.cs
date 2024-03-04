using UnityEngine;
using System.Collections;
public class Card : MonoBehaviour
{
    public string CardName;
    public float attractionRadius = 5f; // Radio de atracci�n
    public float attractionForce = 10f; // Fuerza de atracci�n
    // M�todo para cambiar la posici�n entre el jugador y la carta
    public void Activate()
    {

        switch (CardName)
        {
            case "TpCard":
                // Llama a la funci�n para manejar TpCard
                TpCard();
                Destroy(this.gameObject);
                break;
            case "CartaHole":
                // Llama a la funci�n para manejar BlackHoleCard
                print("First");
                StartCoroutine(AttractCharacters());
                break;
            default:
                // Maneja el caso en el que el nombre de la carta no coincide con las opciones esperadas
                Debug.Log("Error de carta");
                break;
        }

    }
    public void TpCard()
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

        GameObject card = GameObject.FindGameObjectWithTag("Card");

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
    }

    public void BlackHoleCard()
    {
        StartCoroutine(AttractCharacters());
    }
    IEnumerator AttractCharacters()
    {
        float elapsedTime = 0f;
        float duration = 3f; // Duraci�n de la atracci�n

        while (elapsedTime < duration)
        {
            // Encuentra todos los objetos dentro del radio de atracci�n
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attractionRadius);
            foreach (var hitCollider in hitColliders)
            {
                CharacterController controller = hitCollider.GetComponent<CharacterController>();
                if (controller != null)
                {
                    // Calcula la direcci�n hacia el centro de la carta
                    Vector3 direction = (transform.position - controller.transform.position).normalized;
                    // Aplica la fuerza de atracci�n
                    controller.Move(direction * attractionForce * Time.deltaTime);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Espera hasta el pr�ximo frame
        }
        print("finish");
        Destroy(this.gameObject);
    }

    // Utilizado para dibujar un gizmo en el Editor que representa el radio de atracci�n
    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }
}
