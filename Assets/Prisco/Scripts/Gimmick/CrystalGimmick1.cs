using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrystalGimmick1 : MonoBehaviour
{
    [Header("Spawns del cristal")]
    public Transform[] spawnPositions;
    public GameObject turretPrefab1;
    public GameObject turretPrefab2;
    public GameObject turretPrefab3;
    public Transform turretSpawnPosition1;
    public Transform turretSpawnPosition2;
    public Transform turretSpawnPosition3;

    [Header("Condiciones de activación")]
    public int touchMax = 20;
    public int touchCount1 = 1;
    public int touchCount2 = 6;
    public int touchCount3 = 11;

    [Header("Opcional: Cambio de escena y nivel")]
    public bool activarCambioDeEscena = false;
    public string nombreEscenaDestino;
    public bool actualizarNivelDelJugador = false;
    public int nuevoNivelJugador = 1;

    [Header("GameObject a desaparecer")]
    public GameObject objetoADesaparecer;
    public bool desaparecerConEfecto = true;
    public float tiempoDesaparicion = 1f;

    private GameObject player;
    private int touchCount = 0;
    private Transform lastPosition;
    private Renderer objectRenderer;
    private Collider objectCollider;

    void Start()
    {
        Invoke("FindPlayer", 1f);
        lastPosition = transform;
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();
    }

    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            touchCount++;
            StartCoroutine(ChangePosition());

            if ((touchCount == touchCount1) && (turretPrefab1 != null))
            {
                turretPrefab1 = Instantiate(turretPrefab1, turretSpawnPosition1.position, Quaternion.identity);
            }
            else if ((touchCount == touchCount2) && (turretPrefab2 != null))
            {
                turretPrefab2 = Instantiate(turretPrefab2, turretSpawnPosition2.position, Quaternion.identity);
            }
            else if ((touchCount == touchCount3) && (turretPrefab3 != null))
            {
                turretPrefab3 = Instantiate(turretPrefab3, turretSpawnPosition3.position, Quaternion.identity);
            }
            else if (touchCount == touchMax)
            {
                if (turretPrefab1 != null) Destroy(turretPrefab1);
                if (turretPrefab2 != null) Destroy(turretPrefab2);
                if (turretPrefab3 != null) Destroy(turretPrefab3);
                
                StartCoroutine(EjecutarOpcionesFinalesYDestruir());
            }
        }
    }

    private IEnumerator EjecutarOpcionesFinalesYDestruir()
    {
        if (actualizarNivelDelJugador)
        {
            MovimientoJugador mov = player.GetComponent<MovimientoJugador>();
            if (mov != null)
            {
                mov.nivelActual = nuevoNivelJugador;
            }
            else
            {
                Debug.LogWarning("No se encontró MovimientoJugador para actualizar nivel.");
            }
        }

        // Hacer desaparecer el GameObject especificado y esperar a que termine
        if (objetoADesaparecer != null)
        {
            if (desaparecerConEfecto)
            {
                yield return StartCoroutine(DesaparecerConEfecto());
            }
            else
            {
                objetoADesaparecer.SetActive(false);
                Debug.Log($"GameObject {objetoADesaparecer.name} desaparecido instantáneamente.");
            }
        }

        // Solo cambiar escena después de que el objeto haya desaparecido
        if (activarCambioDeEscena && !string.IsNullOrEmpty(nombreEscenaDestino))
        {
            SceneManager.LoadScene(nombreEscenaDestino);
        }
        
        // Destruir este objeto al final
        Destroy(gameObject);
    }

    private IEnumerator DesaparecerConEfecto()
    {
        Debug.Log($"Iniciando desaparición de {objetoADesaparecer.name}");
        
        Renderer objRenderer = objetoADesaparecer.GetComponent<Renderer>();
        Material originalMaterial = null;
        Color originalColor = Color.white;
        
        // Guardar el color original si tiene Renderer
        if (objRenderer != null && objRenderer.material != null)
        {
            originalMaterial = objRenderer.material;
            originalColor = originalMaterial.color;
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < tiempoDesaparicion)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / tiempoDesaparicion);
            
            // Hacer fade del objeto si tiene Renderer
            if (objRenderer != null && originalMaterial != null)
            {
                Color newColor = originalColor;
                newColor.a = alpha;
                objRenderer.material.color = newColor;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Desactivar el objeto al final
        objetoADesaparecer.SetActive(false);
        Debug.Log($"GameObject {objetoADesaparecer.name} desaparecido completamente.");
    }

    private IEnumerator ChangePosition()
    {
        objectRenderer.enabled = false;
        objectCollider.enabled = false;

        yield return new WaitForSeconds(0.5f);

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, spawnPositions.Length);
        } while (spawnPositions[randomIndex] == lastPosition);

        lastPosition = spawnPositions[randomIndex];
        transform.position = spawnPositions[randomIndex].position;

        objectRenderer.enabled = true;
        objectCollider.enabled = true;
    }

    void Update()
    {
        if (touchCount >= touchMax)
        {
            PriscoGuapo();
        }
    }

    void PriscoGuapo()
    {
        //esta pal prisco🖕
    }
}
