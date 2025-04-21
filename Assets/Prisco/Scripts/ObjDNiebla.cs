using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDNiebla : MonoBehaviour
{
    [Header("Área de niebla (Trigger)")]
    public Collider areaDeNiebla;

    [Header("Referencia a ManagerNiebla")]
    public ManagerNiebla managerNiebla;

    [Header("Modo normal")]  
    public float radioInicialNormal = 1f;
    public float radioMaximoNormal = 6f;   
    
    [Header("Modo expansivo")]
    public bool usarModoExpansivo; 
    public float radioInicialExpansivo = 1f;     
    public float maxRadio = 6f;    
    public float incrementoPorGolpe = 1f;

    [Header("Velocidad ambos modos")]
    public float velocidadAjusteRadio = 2f;   

    private bool areaActiva = false;
    private SphereCollider sphereCollider;
    private Coroutine reducirRadioCoroutine;
    private Coroutine aumentarRadioCoroutine;
    private Coroutine ajustarRadioCoroutine;
    private float tiempoSinAtaque = 0f;

    private void Start()
    {
        sphereCollider = areaDeNiebla as SphereCollider;

        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
            sphereCollider.radius = usarModoExpansivo ? radioInicialExpansivo : radioInicialNormal; // Usar el radio inicial según el modo
        }

        FindObjectOfType<FogOfWarManager>().nieblaSources.Add(this);
    }

    private void Update()
    {
        if (usarModoExpansivo && areaActiva)
        {
            tiempoSinAtaque += Time.deltaTime;

            if (tiempoSinAtaque >= 2f && reducirRadioCoroutine == null)
            {
                reducirRadioCoroutine = StartCoroutine(ReducirRadioFluida());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject objeto = other.gameObject;

        if (EsAtaqueDelJugador(objeto))
        {
            tiempoSinAtaque = 0f; // Reinicia el tiempo sin ataque

            if (!areaActiva)
            {
                areaActiva = true;

                if (sphereCollider != null)
                {
                    sphereCollider.enabled = true;
                    sphereCollider.radius = usarModoExpansivo ? radioInicialExpansivo : radioInicialNormal; // Usar el radio inicial según el modo
                }

                if (!usarModoExpansivo && aumentarRadioCoroutine == null)
                {
                    aumentarRadioCoroutine = StartCoroutine(AumentarRadioSuavemente(radioMaximoNormal)); // Usar radio máximo del modo normal
                }
            }

            if (usarModoExpansivo)
            {
                float nuevoRadio = Mathf.Min(sphereCollider.radius + incrementoPorGolpe, maxRadio); // Usar maxRadio para modo expansivo
                if (ajustarRadioCoroutine != null)
                    StopCoroutine(ajustarRadioCoroutine);
                ajustarRadioCoroutine = StartCoroutine(AjustarRadioSuavemente(nuevoRadio));

                if (reducirRadioCoroutine != null)
                {
                    StopCoroutine(reducirRadioCoroutine);
                    reducirRadioCoroutine = null;
                }
            }
        }

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

            if (usarModoExpansivo && sphereCollider != null)
            {
                if (ajustarRadioCoroutine != null)
                    StopCoroutine(ajustarRadioCoroutine);
                ajustarRadioCoroutine = StartCoroutine(AjustarRadioSuavemente(radioInicialExpansivo)); // Reducir suavemente al radio inicial del modo expansivo
            }

            if (reducirRadioCoroutine != null)
            {
                StopCoroutine(reducirRadioCoroutine);
                reducirRadioCoroutine = null;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (areaActiva && other.CompareTag("Player"))
        {
            managerNiebla.isActive = true; // Asegura que la niebla esté activa mientras el jugador esté dentro
        }
    }

    private IEnumerator ReducirRadioFluida()
    {
        while (usarModoExpansivo && sphereCollider != null && sphereCollider.radius > radioInicialExpansivo) // Usar valor público
        {
            sphereCollider.radius = Mathf.MoveTowards(sphereCollider.radius, radioInicialExpansivo, Time.deltaTime * velocidadAjusteRadio); // Usar valores públicos
            yield return null;

            if (tiempoSinAtaque == 0f) // Si el jugador ataca, se detiene la reducción
            {
                reducirRadioCoroutine = null;
                yield break;
            }
        }

        reducirRadioCoroutine = null;
    }

    private IEnumerator AumentarRadioSuavemente(float objetivo)
    {
        while (sphereCollider != null && sphereCollider.radius < objetivo)
        {
            sphereCollider.radius = Mathf.MoveTowards(sphereCollider.radius, objetivo, Time.deltaTime * velocidadAjusteRadio); // Usar valor público
            yield return null;
        }

        aumentarRadioCoroutine = null;
    }

    private IEnumerator AjustarRadioSuavemente(float objetivo)
    {
        while (sphereCollider != null && !Mathf.Approximately(sphereCollider.radius, objetivo))
        {
            sphereCollider.radius = Mathf.MoveTowards(sphereCollider.radius, objetivo, Time.deltaTime * velocidadAjusteRadio); // Usar valor público
            yield return null;
        }

        if (Mathf.Approximately(sphereCollider.radius, (usarModoExpansivo ? radioInicialExpansivo : radioInicialNormal)) && objetivo == (usarModoExpansivo ? radioInicialExpansivo : radioInicialNormal)) // Usar radio inicial según el modo
        {
            sphereCollider.enabled = false;
            areaActiva = false;
        }

        ajustarRadioCoroutine = null;
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
