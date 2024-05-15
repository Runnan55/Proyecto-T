using Unity.VisualScripting;
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

    private string materialActivable = "MaterialActivablePreview"; //Material para la previsualización de dirección de activables.
    private string materialPlace = "MaterialPlacePreview"; //Material para la previsualización de dirección de colocación.
    
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

        }
                // Buscar el Line Renderer en el GameObject "PlayerGround" (hijo del jugador)
        lineRenderer = GetComponentInChildren<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("No se encontró el Line Renderer en el GameObject hijo 'PlayerGround'. Asegúrate de agregarlo.");
        }
        else
        {
            // Desactivar el Line Renderer al inicio (opcional)
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
            
            InventarioPlayer Inventario = GetComponent<InventarioPlayer>();
        if (Inventario.mode == "place" || CartaColocada)
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
                    BaseCard cardComponent = placedObject.GetComponent<BaseCard>();

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
    if (Inventario.mode == "self")
    {
         if (Input.GetMouseButtonDown(1)) 
             {
                SpeedBuff speed = GetComponent<SpeedBuff>();
                StatusManager manager = GetComponent<StatusManager>();
                manager.AddEffect(speed,this.gameObject);
                InventarioPlayer.Instance.UseCard();
             }  
    }

        if (Inventario.mode == "activable") //Recordar que es throw NO CAMBIAR AUN
    {
        if (Input.GetMouseButton(1))
        {
            // Mostrar la previsualización de dirección al activar la carta activable
            ShowDirectionPreviewThrow();
            // Usar la carta activable al soltar el botón
        }
        if (Input.GetMouseButtonUp(1))
        {
            lineRenderer.enabled = false;
            // Ocultar la previsualización de dirección al soltar el botón
            BaseCard cardComponent = GetComponent<BaseCard>();
            cardComponent.Activate();
            InventarioPlayer.Instance.UseCard();
        }
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
        targetPosition.y = player.position.y; // Mantener la altura del jugador
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
    Material material2 = Resources.Load<Material>(materialPlace);
    if (material2 != null)
    {
        lineRenderer.material = material2;
    }
    else
    {
        Debug.LogError("No se pudo cargar el material desde la ruta proporcionada: " + materialPlace);
    }   
    if (lineRenderer != null && currentPreviewInstance != null)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, player.position);
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;

        // Asegúrate de que el segundo punto esté en el mismo plano horizontal que el jugador
        Vector3 previewPosition = currentPreviewInstance.transform.position;
        Vector3 fixedPreviewPosition = new Vector3(previewPosition.x, player.position.y, previewPosition.z);
        lineRenderer.SetPosition(1, fixedPreviewPosition);
        lineRenderer.textureMode = LineTextureMode.Tile;
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
void ShowDirectionPreviewThrow()
{
    // Cargar el material desde la carpeta Resources y asignarlo al Line Renderer
    Material material = Resources.Load<Material>(materialActivable);
    if (material != null)
    {
        lineRenderer.material = material;
    }
    else
    {
        Debug.LogError("No se pudo cargar el material desde la ruta proporcionada: " + materialActivable);
    }    
    if (lineRenderer != null && player != null)
    {
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.textureMode = LineTextureMode.Stretch;

        // Obtener la posición del mouse en el mundo
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Obtener la posición del mouse solo en el plano XZ
            Vector3 targetPosition = hit.point;
            targetPosition.y = player.position.y; // Mantener la altura del jugador

            // Limitar el rango de previsualización
            Vector3 direction = targetPosition - player.position;
            if (direction.magnitude > maxDistance)
            {
                targetPosition = player.position + direction.normalized * maxDistance;
            }

            // Actualizar los puntos del Line Renderer para previsualizar la dirección
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, player.position);

            // Asegúrate de que el segundo punto esté en el mismo plano horizontal que el jugador
            Vector3 fixedTargetPosition = new Vector3(targetPosition.x, player.position.y, targetPosition.z);
            lineRenderer.SetPosition(1, fixedTargetPosition);
        }
    }
}



}
