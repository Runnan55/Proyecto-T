using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilBruja : MonoBehaviour
{
    public float speed = 10f;
  
    public float damage = 5f;
    public float explosionRadius = 3f; // Radio del área de daño
    public float delayBeforeDamage = 1f; // Tiempo de espera antes de hacer daño

    public GameObject impactAreaPrefab; // Prefab para el área de impacto
    private GameObject impactAreaInstance; // Instancia del área de impacto
    private Renderer impactAreaRenderer; // Renderer del área de impacto
    private bool isDealingDamage = false; // Control para daño continuo

    private bool hasExploded = false;
    private Transform playerTransform;
    private Vector3 initialTargetPosition; // Posición inicial del jugador al disparar

    [SerializeField] private Life playerLife;
    [SerializeField] private MovimientoJugador movimientoJugador;

    [Range(0f, 1f)] public float impactAreaTransparency = 0.3f; // Transparencia ajustable desde el inspector

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLife = player.GetComponent<Life>();
            movimientoJugador = player.GetComponent<MovimientoJugador>();
            playerTransform = player.transform;

            // Establecer la posición inicial del objetivo en los pies del jugador
            initialTargetPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

            // Crear la instancia del área de impacto pero desactivarla inicialmente
            if (impactAreaPrefab != null)
            {
                impactAreaInstance = Instantiate(impactAreaPrefab, transform.position, Quaternion.identity); // Generar en la posición de la bala
                impactAreaInstance.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, explosionRadius * 2); // Escalar como esfera
                impactAreaInstance.SetActive(false); // Desactivar el área de impacto

                // Obtener el renderer del área de impacto
                impactAreaRenderer = impactAreaInstance.GetComponent<Renderer>();
            }
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    void Update()
    {
        if (!hasExploded && playerTransform != null)
        {
            // Verificar si el jugador está en el área de esquiva
            float timeScale = MovimientoJugador.isInDodgeArea ? MovimientoJugador.bulletTimeScale : 1f;

            // Mover el proyectil hacia la posición inicial de los pies del jugador
            transform.position = Vector3.MoveTowards(
                transform.position, 
                initialTargetPosition, 
                speed * timeScale * Time.deltaTime
            );

            // Actualizar la posición del área de impacto para que siga a la bala
            if (impactAreaInstance != null)
            {
                impactAreaInstance.transform.position = transform.position; // Centro en la posición de la bala
                impactAreaInstance.transform.localScale = new Vector3(
                    explosionRadius * 2 * timeScale, 
                    explosionRadius * 2 * timeScale, 
                    explosionRadius * 2 * timeScale
                ); // Escalar según el timeScale
            }

            // Verificar si el proyectil ha llegado a la posición objetivo
            if (Vector3.Distance(transform.position, initialTargetPosition) < 0.1f)
            {
                if (impactAreaInstance != null)
                {
                    impactAreaInstance.SetActive(true); // Activar el área de impacto
                    StartCoroutine(ChangeImpactAreaColor());
                }
                StartCoroutine(ExplodeAfterDelay());
                hasExploded = true;
            }
        }
    }

    private IEnumerator ChangeImpactAreaColor()
    {
        if (impactAreaRenderer != null)
        {
            float elapsedTime = 0f;
            float colorChangeDuration = delayBeforeDamage;

            // Asegurar que el material permita transparencia
            impactAreaRenderer.material.SetFloat("_Mode", 3); // Modo transparente
            impactAreaRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            impactAreaRenderer.material.renderQueue = 3000;

            while (elapsedTime < colorChangeDuration)
            {
                float t = elapsedTime / colorChangeDuration;
                Color color = Color.Lerp(new Color(1, 1, 1, impactAreaTransparency), new Color(1, 0, 0, impactAreaTransparency), t); // Usar transparencia ajustable
                impactAreaRenderer.material.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            impactAreaRenderer.material.color = new Color(1, 0, 0, impactAreaTransparency); // Asegurarse de que sea rojo transparente al final
        }
    }

    private IEnumerator ExplodeAfterDelay()
    {
        StartCoroutine(ChangeImpactAreaColor()); // Iniciar el cambio de color al mismo tiempo
        yield return new WaitForSeconds(delayBeforeDamage);

        // Hacer daño en un área
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && playerLife != null)
            {
                playerLife.ModifyTime(-damage);
            }
        }

        // Destruir la instancia del área de impacto
        if (impactAreaInstance != null)
        {
            Destroy(impactAreaInstance);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar el área de daño en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("BTCollider"))
        {
            if (movimientoJugador != null)
            {
                movimientoJugador.CountBTProjectiles();
            }
            else
            {
                Debug.LogError("movimientoJugador is null!");
            }
        }
        else if (other.CompareTag("DodgeArea"))
        {
            MovimientoJugador.isInDodgeArea = true; // Activar el estado de área de esquiva
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DodgeArea"))
        {
            MovimientoJugador.isInDodgeArea = false; // Desactivar el estado de área de esquiva
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Aplicar daño continuo mientras el jugador esté en el área
        if (isDealingDamage && other.CompareTag("Player") && playerLife != null)
        {
            playerLife.ModifyTime(-damage * Time.deltaTime); // Daño continuo basado en el tiempo
        }
    }

    private void OnDestroy()
    {
        // Asegurarse de destruir el área de impacto si el proyectil es destruido
        if (impactAreaInstance != null)
        {
            Destroy(impactAreaInstance);
        }
    }
}
