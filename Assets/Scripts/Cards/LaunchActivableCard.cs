using UnityEngine;

public class LaunchActivableCard : BaseCard
{
    public GameObject objectToLaunch;  // Objeto a lanzar
    public float launchSpeed = 25f;    // Velocidad de lanzamiento
    public float altura = 2f; // Altura del lanzamiento

    private Camera mainCamera;         // Cámara principal
    private Transform playerTransform; // Transform del jugador
    

    void Start()
    {
        //Parte inicial de cartas
        objectToLaunch = Resources.Load<GameObject>("Fireball");
                if (objectToLaunch == null)
        {
            Debug.LogError("No se pudo cargar el prefab Fireball desde Resources");
        }
        mainCamera = Camera.main; // Asigna la cámara principal
        playerTransform = this.transform; // Asigna el transform del jugador
    }
    public override void Activate()
    {
        LaunchObject();
    }
    void Update()
    {

    }

    void LaunchObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Calcula la dirección del lanzamiento pero modifica la altura desde la que se lanza
            Vector3 launchDirection = new Vector3(hit.point.x, playerTransform.position.y, hit.point.z) - playerTransform.position;
            // Instancia el objeto en la posición del jugador ajustada con la altura
            GameObject launchedObject = Instantiate(objectToLaunch, new Vector3(playerTransform.position.x, playerTransform.position.y + altura, playerTransform.position.z), Quaternion.identity);
            Rigidbody rb = launchedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = launchDirection.normalized * launchSpeed;
            }
        }
    }

   
}
