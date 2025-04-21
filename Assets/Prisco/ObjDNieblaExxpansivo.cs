using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDNieblaExxpansivo : MonoBehaviour
{
    [Header("Área de niebla (Trigger)")]
    public Collider areaDeNiebla;

    [Header("Referencia a ManagerNiebla")]
    public ManagerNiebla managerNiebla;

    [Header("Radio inicial del área de niebla")]
    public float radioAreaDeNiebla = 1f;

    [Header("Aumentos máximos de radio")]
    public int maxAumentos = 5;

    private bool areaActiva = false;
    private SphereCollider sphereCollider;
    private int aumentosRealizados = 0;

    private void Start()
    {
        
        sphereCollider = areaDeNiebla as SphereCollider;

        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
            sphereCollider.radius = radioAreaDeNiebla;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject objeto = other.gameObject;

        // Si recibe un ataque del jugador
        if (EsAtaqueDelJugador(objeto))
        {
            if (!areaActiva)
            {
                areaActiva = true;

                if (sphereCollider != null)
                    sphereCollider.enabled = true;
            }

            if (aumentosRealizados < maxAumentos)
            {
                sphereCollider.radius += 1f;
                aumentosRealizados++;
            }
        }

        // Si entra el jugador
        if (areaActiva && other.CompareTag("Player"))
        {
            managerNiebla.isActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (areaActiva && other.CompareTag("Player"))
        {
            managerNiebla.isActive = false;
        }
    }

    private bool EsAtaqueDelJugador(GameObject objeto)
    {
        return objeto.name == "AtaqueL1" ||
               objeto.name == "AtaqueL2" ||
               objeto.name == "AtaqueL3" ||
               objeto.name == "CollidersAtaqueP" ||
               objeto.CompareTag("Boomerang");
    }

    private void OnDrawGizmosSelected()
    {
        if (areaDeNiebla is SphereCollider gizmoSphere)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            float scaledRadius = gizmoSphere.radius * Mathf.Max(
                gizmoSphere.transform.lossyScale.x,
                gizmoSphere.transform.lossyScale.y,
                gizmoSphere.transform.lossyScale.z
            );
            Gizmos.DrawSphere(gizmoSphere.transform.position + gizmoSphere.center, scaledRadius);
        }
    }

    public bool AreaActiva()
    {
        return areaActiva;
    }
}
