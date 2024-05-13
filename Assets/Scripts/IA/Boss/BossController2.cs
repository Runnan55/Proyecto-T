using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController2 : MonoBehaviour
{
    public GameObject cubePrefab; // Prefab del cubo para el primer patrón de ataque
    public GameObject player;


    public float attackCooldown = 3f; // Tiempo entre ataques
    public float circularAttackRadius = 5f; // Radio de las zonas de ataque circular

    public Transform[] attackZones; // Array de transforms para las posiciones de las zonas de ataque
    public float damageRadius;
    public float timeBetweenPatterns = 2f; // Tiempo entre cada repetición de patrones

    public LayerMask playerLayer;
    float damageAmount = 10f;
    private float lastAttackTime;

    void Update()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Realizar un ataque aleatorio
            int attackPattern = Random.Range(1, 3);
            switch (attackPattern)
            {
                case 1:
                    ExecuteFrontalAttack();
                    break;
                case 2:
                    ExecuteCircularAttack();
                    break;
            }

            lastAttackTime = Time.time;
        }
    }

    void ExecuteFrontalAttack()
    {
        // Generar dos puntos aleatorios en frente del jefe
        Vector3 attackPoint1 = transform.position + transform.forward * 5f;
        Vector3 attackPoint2 = transform.position + transform.forward * 7f;

        // Instanciar los cubos en los puntos de ataque
        Instantiate(cubePrefab, attackPoint1, Quaternion.identity);
        Instantiate(cubePrefab, attackPoint2, Quaternion.identity);
    }

    void ExecuteCircularAttack()
    {
        StartCoroutine(Pattern1Routine());

    }


    IEnumerator PreviewAndExecutePattern1()
    {
        // Previsualización del ataque 1 segundo antes
        Debug.Log("Previewing Pattern 1");
        yield return new WaitForSeconds(1f);

        // Ejecuta el ataque
        Debug.Log("Executing Pattern 1");
        foreach (var point in attackZones)
        {
            Collider[] hitColliders = Physics.OverlapSphere(point.position, damageRadius, playerLayer);

            foreach (Collider collider in hitColliders)
            {
                // Comprobar si el objeto colisionado es el jugador
                if (collider.gameObject == player)
                {
                    // Aplicar daño al jugador
                    Debug.Log("Player hit by boss!");

                    // Acceder al componente Life del jugador
                    Life playerLife = collider.gameObject.GetComponent<Life>();
                    if (playerLife != null)
                    {
                        // Reducir la vida del jugador (tiempo)
                        playerLife.ModifyTime(-damageAmount); // Cambia el valor de -60 según sea necesario
                    }
                }
            }
        }
    }
    IEnumerator Pattern1Routine()
    {
        while (true)
        {
            Debug.Log("Starting Pattern 1");
            StartCoroutine(PreviewAndExecutePattern1()); // Inicia la previsualización y el ataque
            yield return new WaitForSeconds(timeBetweenPatterns); // Espera antes de pasar al siguiente patrón
        }
    }

}