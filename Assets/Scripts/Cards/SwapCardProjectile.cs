using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCardProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject playerParent = GameObject.FindGameObjectWithTag("Player");
            CharacterController controller = player.GetComponent<CharacterController>();
            
            if (controller != null) controller.enabled = false;
            
            // Llama a ResetStart antes de la teletransportación
            ResetStart(other);

            Transform playerGroundTransform = playerParent?.transform.Find("PlayerGround");
            Vector3 offset = playerParent.transform.position - playerGroundTransform.position;

            Vector3 tempPosition = playerParent.transform.position;
            playerParent.transform.position = other.transform.position + offset;
            other.transform.position = tempPosition;

            // Llama a ResetFinish después de la teletransportación
            ResetFinish(other);
            if (controller != null) controller.enabled = true;
            Destroy(gameObject); // Destruye el objeto que golpea
        }
        else if (other.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject); // Destruye el objeto que golpea
        }     
    }

    void ResetStart(Collider hitCollider)
    {
        ZombiEnemy enemy = hitCollider.GetComponent<ZombiEnemy>();
        CE charger = hitCollider.GetComponent<CE>();
        ArqueroAnim distance = hitCollider.GetComponent<ArqueroAnim>();
        EnemyTpA tp = hitCollider.GetComponent<EnemyTpA>();

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

    void ResetFinish(Collider hitCollider)
    {
        Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
        ZombiEnemy enemy = hitCollider.GetComponent<ZombiEnemy>();
        CE charger = hitCollider.GetComponent<CE>();
        ArqueroAnim distance = hitCollider.GetComponent<ArqueroAnim>();
        EnemyTpA tp = hitCollider.GetComponent<EnemyTpA>();

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
