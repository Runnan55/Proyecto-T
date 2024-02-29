using UnityEngine;

public class ItemPlacer : MonoBehaviour
{
    public GameObject itemPrefab; // Prefab del item final
    public GameObject previewPrefab; // Prefab de la previsualizaci�n permitida
    public GameObject invalidPreviewPrefab; // Prefab de la previsualizaci�n no permitida
    public LayerMask placementLayer; // Layer Mask para el suelo
    public LayerMask obstructionLayer; // Layer Mask para detectar obstrucciones
    public Transform player; // Referencia p�blica al jugador

    private GameObject currentPreviewInstance; // Instancia actual de la previsualizaci�n
    private GameObject placedObject; // Referencia al objeto colocado
    private bool isPreviewValid = false; // Indica si la previsualizaci�n actual es v�lida
    private LineRenderer lineRenderer; // LineRenderer para dibujar el trail
    private bool isPlacementMode = true; // Controla si el modo actual es de colocaci�n

    void Start()
    {
        // Intenta obtener el LineRenderer del jugador, si no existe, cr�alo.
        lineRenderer = player.gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = player.gameObject.AddComponent<LineRenderer>();
            // Configura el LineRenderer aqu�
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Aseg�rate de asignar un material adecuado
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Al presionar el bot�n derecho
        {
            if (isPlacementMode)
            {
                if (currentPreviewInstance == null)
                {
                    CreateOrUpdatePreview(previewPrefab);
                }
            }
            else
            {
                Card cardComponent = placedObject.GetComponent<Card>();
                if (cardComponent != null)
                {
                    cardComponent.Activate();
                    // Acci�n de activaci�n, eliminar el objeto colocado y volver al modo de colocaci�n
                    Debug.Log("Activaci�n");
                }
                if (placedObject != null)
                {
                    //Destroy(placedObject); // Elimina el objeto colocado
                    placedObject = null;
                }
                isPlacementMode = true; // Vuelve al modo de colocaci�n
            }
        }

        if (isPlacementMode && currentPreviewInstance != null)
        {
            UpdatePreviewPositionAndStatus();
            DrawTrailToPreview(); // Dibuja el trail hacia el prefab
        }
        else if (lineRenderer != null && !Input.GetMouseButton(1))
        {
            lineRenderer.enabled = false; // Oculta el trail si no hay previsualizaci�n
        }

        if (Input.GetMouseButtonUp(1) && isPlacementMode) // Intentar colocar el item al soltar, solo si est� en modo de colocaci�n
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
            placedObject = Instantiate(itemPrefab, currentPreviewInstance.transform.position, Quaternion.identity);
            isPlacementMode = false; // Desactiva el modo de colocaci�n despu�s de colocar un objeto
        }
        if (currentPreviewInstance != null)
        {
            Destroy(currentPreviewInstance);
            currentPreviewInstance = null;
        }
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Oculta el trail despu�s de colocar el item
        }
    }
}
