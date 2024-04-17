using UnityEngine;
using System.Collections;
public class Card : MonoBehaviour
{
    public string CardName;
    public float attractionRadius = 5f; // Radio de atracci�n
    public float attractionForce = 10f; // Fuerza de atracci�n
    public float damping = 5f; // Amortiguación inicial para reducir el impulso lateral
    public float duration = 3f; // Duración de la atracción

    public GameObject Zona;
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
    }
    public void BlackHoleCard()
    {
        StartCoroutine(AttractCharacters());
    }

    IEnumerator AttractCharacters()
    {
        float elapsedTime = 0f;
        Zona.SetActive(true);
        while (elapsedTime < duration)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attractionRadius);
            foreach (var hitCollider in hitColliders)
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    ResetStart();
                    Vector3 toCenter = (transform.position - rb.transform.position);
                    Vector3 direction = toCenter.normalized;
                    // Calcula la velocidad actual hacia el centro
                    float currentSpeedTowardsCenter = Vector3.Dot(rb.velocity, direction);
                    // Calcula la fuerza de amortiguación para reducir el movimiento lateral
                    Vector3 dampingForce = -rb.velocity + direction * currentSpeedTowardsCenter;
                    // Ajusta la fuerza de atracción y la amortiguación basándose en el tiempo restante
                    float attractionForceAdjusted = attractionForce * (1 - (elapsedTime / duration));
                    float dampingAdjusted = damping * (1 - (elapsedTime / duration));
                    // Aplica la fuerza de atracción ajustada junto con la fuerza de amortiguación ajustada
                    rb.AddForce((direction * attractionForceAdjusted + dampingForce * dampingAdjusted) * Time.deltaTime, ForceMode.VelocityChange);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        ResetFinish();
        Destroy(this.gameObject);
        // Aquí puedes agregar lógica adicional si necesitas realizar alguna acción después de que finalice la atracción
    }
    void ResetStart()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attractionRadius);
        foreach (var hitCollider in hitColliders)
        {
            SimpleEnemy enemy = hitCollider.GetComponent<SimpleEnemy>();
            ChargeEnemy charger = hitCollider.GetComponent<ChargeEnemy>();
            DistanceEnemy distance = hitCollider.GetComponent<DistanceEnemy>();
            TpEnemy tp = hitCollider.GetComponent<TpEnemy>();
            if (enemy != null)
            {
                // Reinicia la velocidad lineal y angular
                enemy.DesactiveNavMesh();
            }
            if (distance != null)
            {
                // Reinicia la velocidad lineal y angular
                distance.DesactiveNavMesh();
            }
            if (tp != null)
            {
                // Reinicia la velocidad lineal y angular
                tp.DesactiveNavMesh();
            }
            if (charger != null)
            {
                // Reinicia la velocidad lineal y angular
                charger.DesactivarMovimientos();
            }
           
        }
    }
    void ResetFinish()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attractionRadius);
        foreach (var hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            SimpleEnemy enemy = hitCollider.GetComponent<SimpleEnemy>();
            ChargeEnemy charger = hitCollider.GetComponent<ChargeEnemy>();
            DistanceEnemy distance = hitCollider.GetComponent<DistanceEnemy>();
            TpEnemy tp = hitCollider.GetComponent<TpEnemy>();
            if (rb != null)
            {
                // Reinicia la velocidad lineal y angular
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
                        if (enemy != null)
            {
                // Reinicia la velocidad lineal y angular
                enemy.ActiveNavMesh();
            }
                if (distance != null)
            {
                // Reinicia la velocidad lineal y angular
                distance.ActiveNavMesh();
            }
            if (tp != null)
            {
                // Reinicia la velocidad lineal y angular
                tp.ActiveNavMesh();
            }
            if (charger != null)
            {
                // Reinicia la velocidad lineal y angular
                charger.ReactivarMovimientos();
            }
        }
    }
    // Utilizado para dibujar un gizmo en el Editor que representa el radio de atracci�n
    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }
}
