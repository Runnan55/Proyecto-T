using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleCard : BaseCard
{
    public float attractionRadius = 5f; // Radio de atracci�n
    public float attractionForce = 10f; // Fuerza de atracci�n
    public float damping = 5f; // Amortiguación inicial para reducir el impulso lateral
    public float duration = 3f; // Duración de la atracción

    public GameObject Zona;
    // Start is called before the first frame update
 public override void Activate()
    {
        BlackHole();
    }
        public void BlackHole()
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
