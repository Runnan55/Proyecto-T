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
    private bool isInBulletTime = false; // Nuevo campo para rastrear el estado del tiempo bala

    private void Awake()
    {
        cloneRenderer = GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(ChangeColorAndShoot());
        enemyRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        // Verificar cambios en el tiempo bala y actualizar color si es necesario
        bool currentBulletTime = MovimientoJugador.bulletTimeScale < 1;
        if (currentBulletTime != isInBulletTime)
        {
            isInBulletTime = currentBulletTime;
            ForceUpdateColor();
        }
    }

    // Método público para forzar la actualización del color
    public void ForceUpdateColor()
    {
        if (enemyRenderer == null) return;

        if (MovimientoJugador.bulletTimeScale < 1)
        {
            // Durante el tiempo bala, cambiar a rojo
            enemyRenderer.material.color = Color.red;
        }
        else
        {
            // Tiempo normal, usar el color original del clon
            if (cloneRenderer != null)
            {
                enemyRenderer.material.color = cloneRenderer.material.color;
            }
        }
    }

    private IEnumerator ChangeColorAndShoot()
    {
        bool isPreShooting = true;
        Color originalColor = cloneRenderer.material.color;
        Color secondaryColor = new Color(1.0f, 0.5f, 0.0f);

        while (isPreShooting)
        {
            // Verificar si el objeto aún existe antes de continuar
            if (cloneRenderer == null || this == null)
            {
                yield break;
            }

            waitTimer -= Time.deltaTime * MovimientoJugador.bulletTimeScale;
            float lerpFactor = 1 - (waitTimer / preShootDelay);

            // Verificar nuevamente antes de acceder al material
            if (cloneRenderer != null)
            {
                cloneRenderer.material.color = Color.Lerp(originalColor, secondaryColor, lerpFactor);
            }

            if (waitTimer <= 0)
            {
                // Verificar antes de restaurar el color
                if (cloneRenderer != null)
                {
                    cloneRenderer.material.color = originalColor;
                }

                isPreShooting = false;

                // Instanciar 3 prefabs de balas con un delay entre cada una
                for (int i = 0; i < 3; i++)
                {
                    // Verificar si el objeto aún existe antes de disparar
                    if (bulletPrefab != null && bulletSpawnPoint != null && this != null)
                    {
                        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                        yield return new WaitForSeconds(shootDelay * MovimientoJugador.bulletTimeScale);
                    }
                    else
                    {
                        yield break; // Salir si el objeto ha sido destruido
                    }
                }
            }
            yield return null;
        }
    }
}
