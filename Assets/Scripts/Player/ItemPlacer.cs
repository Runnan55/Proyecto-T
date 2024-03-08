using UnityEngine;

public class ItemPlacer : MonoBehaviour
{
    public GameObject previewPrefab; // Prefab de la previsualización permitida
    public GameObject invalidPreviewPrefab; // Prefab de la previsualización no permitida
    public LayerMask placementLayer; // Layer Mask para el suelo
    public LayerMask obstructionLayer; // Layer Mask para detectar obstrucciones
    public Transform player; // Referencia pública al jugador

    private GameObject currentPreviewInstance; // Instancia actual de la previsualización
    private GameObject placedObject; // Referencia al objeto colocado
    private bool isPreviewValid = false; // Indica si la previsualización actual es válida
    private LineRenderer lineRenderer; // LineRenderer para dibujar el trail
    private bool isPlacementMode = true; // Controla si el modo actual es de colocación

    public GameObject CartaTp; // Referencia al jugador

    public GameObject CartaHole; // Referencia al jugador

    public bool CartaColocada = false;

    public float maxDistance = 10f; // Rango máximo de previsualización
    void Start()
    {
        // Intenta obtener el LineRenderer del jugador, si no existe, créalo.
        lineRenderer = player.gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = player.gameObject.AddComponent<LineRenderer>();
            // Configura el LineRenderer aquí
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Asegúrate de asignar un material adecuado
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Al presionar el botón derecho
        {
            string activeCardName = InventarioPlayer.Instance.GetCurrentCardName(); // Obtiene el nombre de la carta activa
            if (activeCardName != "Empty" && !CartaColocada) // Verifica que el slot activo no esté vacío
            {
                if (isPlacementMode)
                {
                    if (currentPreviewInstance == null)
                    {
                        CreateOrUpdatePreview(previewPrefab);
                        
                    }
                }
            
            }
            if (CartaColocada)
                {
                    if (placedObject != null)
                    {
                    Card cardComponent = placedObject.GetComponent<Card>();

                    if (cardComponent != null)
                    {
                        cardComponent.Activate();
                        // Acción de activación, eliminar el objeto colocado y volver al modo de colocación
                        Debug.Log("Activación");
                    }
                    if (placedObject != null)
                    {
                        //Destroy(placedObject); // Elimina el objeto colocado
                        placedObject = null;
                    }
                    isPlacementMode = true; // Vuelve al modo de colocación
                    CartaColocada = false;
                    }
                }
        }

        if (isPlacementMode && currentPreviewInstance != null)
        {
            UpdatePreviewPositionAndStatus();
            DrawTrailToPreview(); // Dibuja el trail hacia el prefab
        }
        else if (lineRenderer != null && !Input.GetMouseButton(1))
        {
            lineRenderer.enabled = false; // Oculta el trail si no hay previsualización
        }

        if (Input.GetMouseButtonUp(1) && isPlacementMode) // Intentar colocar el item al soltar, solo si está en modo de colocación
        {
            PlaceItemAndClearPreview();
        }
    }

    void CreateOrUpdatePreview(GameObject prefab)
    {
        if (currentPreviewInstance == null || currentPreviewInstance.GetComponentInChildren<MeshRenderer>().gameObject != prefab)
        {
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            if (currentPreviewInstance != null)
            {
                position = currentPreviewInstance.transform.position;
                rotation = currentPreviewInstance.transform.rotation;
                Destroy(currentPreviewInstance);
            }
            currentPreviewInstance = Instantiate(prefab, position, rotation);
        }
    }

void UpdatePreviewPositionAndStatus()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer))
    {
        Vector3 targetPosition = hit.point;
        Vector3 directionFromPlayer = targetPosition - player.position;

        // Limitar el rango de previsualización
        if (directionFromPlayer.magnitude > maxDistance)
        {
            directionFromPlayer = directionFromPlayer.normalized * maxDistance; // 10 es el rango máximo
            targetPosition = player.position + directionFromPlayer;
        }

        currentPreviewInstance.transform.position = targetPosition;
        bool isObstructed = Physics.CheckSphere(targetPosition, 0.5f, obstructionLayer);
        GameObject prefabToUse = isObstructed ? invalidPreviewPrefab : previewPrefab;
        if (isObstructed != isPreviewValid || currentPreviewInstance.GetComponentInChildren<MeshRenderer>().gameObject != prefabToUse)
        {
            isPreviewValid = !isObstructed;
            CreateOrUpdatePreview(prefabToUse);
        }
    }
}

    void DrawTrailToPreview()
    {
        if (lineRenderer != null && currentPreviewInstance != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, player.position);
            lineRenderer.SetPosition(1, currentPreviewInstance.transform.position);
        }
    }

void PlaceItemAndClearPreview()
{
    if (isPreviewValid && currentPreviewInstance != null)
    {
        // Asegurar que la posición de colocación esté dentro del rango máximo permitido
        Vector3 placementPosition = currentPreviewInstance.transform.position;
        Vector3 directionFromPlayer = placementPosition - player.position;
        if (directionFromPlayer.magnitude > maxDistance)
        {
            directionFromPlayer = directionFromPlayer.normalized * maxDistance; // 10 es el rango máximo
            placementPosition = player.position + directionFromPlayer;
        }

        string activeCardName = InventarioPlayer.Instance.GetCurrentCardName();
        GameObject itemPrefab = null;

        switch (activeCardName)
        {
            case "TpCard":
                itemPrefab = CartaTp;
                break;
            case "CartaHole":
                itemPrefab = CartaHole;
                break;
            // Añade más casos según sea necesario
            default:
                Debug.Log("No se encontró una carta válida o el slot está vacío.");
                return;
        }
        CartaColocada = true;
        placedObject = Instantiate(itemPrefab, placementPosition, Quaternion.identity);
        isPlacementMode = false;
        InventarioPlayer.Instance.UseCard();
    }
    if (currentPreviewInstance != null)
    {
        Destroy(currentPreviewInstance);
        currentPreviewInstance = null;
    }
    if (lineRenderer != null)
    {
        lineRenderer.enabled = false;
    }
}
}
