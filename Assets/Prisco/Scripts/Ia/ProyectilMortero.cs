using System.Collections;
using UnityEngine;

public class ProyectilMortero : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isProjectileInAir = false; 
    private GameObject impactAreaInstance;

    public GameObject impactAreaPrefab; 
    public GameObject secondaryProjectilePrefab; // Prefab del proyectil secundario

    private Life playerLife;
    private MovimientoJugador movimientoJugador;
    public float damage = 5f;
    public float speed = 10f;

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * MovimientoJugador.bulletTimeScale * Time.deltaTime);
    }

        void Awake()
    {        
       playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>();
    }


    public void Launch(Vector3 target, float duration)
    {
        targetPosition = target;
        isProjectileInAir = true;

        if (impactAreaPrefab != null)
        {
            impactAreaInstance = Instantiate(impactAreaPrefab, targetPosition, Quaternion.identity);         
            impactAreaInstance.SetActive(true);
        }

        StartCoroutine(LerpProjectile(targetPosition, duration));
    }

    private IEnumerator LerpProjectile(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.y += Mathf.Sin(t * Mathf.PI) * 7f; // curvatura del proyectil
            transform.position = currentPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isProjectileInAir = false; 

        // Verificar si el jugador está dentro del área de impacto
        if (impactAreaInstance != null)
        {
            Collider[] colliders = Physics.OverlapSphere(targetPosition, impactAreaInstance.transform.localScale.x / 2);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    Debug.Log("Player detected in impact area.");
                    if (playerLife != null)
                    {
                        playerLife.ModifyTime(-damage);
                        Debug.Log("Damage applied to player: " + damage);
                    }
                    else
                    {
                        Debug.LogError("playerLife is not initialized.");
                    }
                }
            }

            Destroy(impactAreaInstance); // Destruir el área de impacto después de aplicar el daño
        }

        // Instanciar proyectiles secundarios en las direcciones cardinales
        InstantiateSecondaryProjectiles(targetPosition);

        Destroy(gameObject); // Destruir el proyectil después de hacer daño
    }

    private void InstantiateSecondaryProjectiles(Vector3 position)
    {
        if (secondaryProjectilePrefab != null)
        {
            Vector3[] directions = new Vector3[]
            {
                Vector3.forward, 
                Vector3.back,  
                Vector3.right,   
                Vector3.left     
            };

            foreach (Vector3 direction in directions)
            {
                Vector3 offsetPosition = position + direction * 3f + new Vector3(0, 2f, 0);
                Instantiate(secondaryProjectilePrefab, offsetPosition, Quaternion.LookRotation(direction));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Projectile collided with player.");
            if (playerLife != null)
            {
                playerLife.ModifyTime(-damage);
                Debug.Log("Damage applied to player: " + damage);
                MovimientoJugador.isInDodgeArea = false;
            }
            else
            {
                Debug.LogError("playerLife is not initialized.");
            }
        }
    }

    
}
