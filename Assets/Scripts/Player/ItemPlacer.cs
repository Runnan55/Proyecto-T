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

    public GameObject CartaTp; // Referencia al prefab de la carta de teletransporte
    public GameObject CartaHole; // Referencia al prefab de la carta agujero

    public bool CartaColocada = false;

    void Start()
    {
        lineRenderer = player.gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = player.gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Al presionar el botón derecho
        {
            string activeCardName = InventarioPlayer.Instance.GetCurrentCardName(); // Obtiene el nombre de la carta activa
            if (activeCardName != "Empty" && !CartaColocada) // Verifica que el slot activo no esté vacío y no se haya colocado una carta
            {
                CreateOrUpdatePreview(previewPrefab);
            }
            else if (CartaColocada) // Si ya hay una carta colocada, intenta activarla
            {
                ActivatePlacedCard();
            }
        }

        if (isPlacementMode && currentPreviewInstance != null)
        {
            UpdatePreviewPositionAndStatus();
            DrawTrailToPreview();
        }
        else if (lineRenderer != null)
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
            currentPreviewInstance.transform.position = hit.point;
            bool isObstructed = Physics.CheckSphere(hit.point, 0.5f, obstructionLayer);
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
                default:
                    Debug.Log("No se encontró una carta válida o el slot está vacío.");
                    break; // Sale de la función si no se quiere colocar nada
            }

            if (itemPrefab != null)
            {
                placedObject = Instantiate(itemPrefab, currentPreviewInstance.transform.position, Quaternion.identity);
                CartaColocada = true; // Marca que hay una carta colocada
                InventarioPlayer.Instance.UseCard(); // Usa la carta activa
                isPlacementMode = false; // Desactiva el modo de colocación
            }
        }

        if (currentPreviewInstance != null)
        {
            Destroy(currentPreviewInstance); // Limpia la previsualización siempre
            currentPreviewInstance = null;
        }
        lineRenderer.enabled = false; // Oculta el trail después de intentar colocar el item
    }

    void ActivatePlacedCard()
    {
        // Aquí asumimos que tu componente 'Card' tiene un método 'Activate()'
        if (placedObject != null)
        {
            Card cardComponent = placedObject.GetComponent<Card>();
            if (cardComponent != null)
            {
                cardComponent.Activate();
                Debug.Log("Carta Activada");
                // Realiza cualquier otra limpieza necesaria después de activar la carta
            }
            placedObject = null; // Elimina la referencia al objeto colocado
            CartaColocada = false; // Restablece para permitir nuevas colocaciones
            isPlacementMode = true; // Reactiva el modo de colocación para permitir colocar más cartas
        }
    }
}
