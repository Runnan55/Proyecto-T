using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CloneBehavior : EnemyLife
{
    public GameObject bulletPrefab; // Prefab de las balas
    public Transform bulletSpawnPoint; // Punto de spawn de las balas
    public float shootDelay = 1f; // Velocidad entre cada disparo
    public float preShootDelay = 1.5f; // Duración del pre-disparo
    public float waitTimer = 1.5f; // Duración del cambio de color
    public static Renderer cloneRenderer;
    private NavMeshAgent agent;
    public Vector3 lastPosition; // Añadir esta línea para rastrear la última posición del clon
    private Renderer enemyRenderer;

    
    

    private void Awake()
    {
        cloneRenderer = GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(ChangeColorAndShoot());
         enemyRenderer = GetComponent<Renderer>();
    }

    private IEnumerator ChangeColorAndShoot()
    {
        bool isPreShooting = true; // Indicador de pre-disparo
        Color originalColor = cloneRenderer.material.color; // Guardar el color original
        Color secondaryColor = new Color(1.0f, 0.5f, 0.0f); // Color secundario naranja

        while (isPreShooting)
        {
            waitTimer -= Time.deltaTime;
            float lerpFactor = 1 - (waitTimer / preShootDelay); 
            cloneRenderer.material.color = Color.Lerp(originalColor, secondaryColor, lerpFactor); // Interpolar entre el color original y el secundario

            if (waitTimer <= 0)
            {
                cloneRenderer.material.color = originalColor; // Restaurar el color original
                isPreShooting = false; 

                // Instanciar 3 prefabs de balas con un delay entre cada una
                for (int i = 0; i < 3; i++)
                {
                    Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                    yield return new WaitForSeconds(shootDelay); 
                }           
            }
            yield return null;
        }
    }
}
