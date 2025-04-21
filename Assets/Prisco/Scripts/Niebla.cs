using UnityEngine;

public class Niebla : MonoBehaviour
{
    public GameObject m_fogOfWarPlane; // El objeto que tiene la niebla
    public float m_radius = 5f; // El radio de la disipación de niebla
    public LayerMask capaNieblaLayer; // La capa que representa la niebla
    public bool despideNiebla = false; // Indicador de si el objeto está despidiendo niebla
    private float m_radiusSqr { get { return m_radius * m_radius; } }

    private MeshRenderer m_meshRenderer;
    private Material m_material;
    private Color m_originalColor;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (despideNiebla)
        {
            // Si está activado, detectar colisiones y disipar la niebla
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_radius);
            foreach (var collider in colliders)
            {
                if (IsInFogLayer(collider.gameObject))
                {
                    Vector3 collisionPoint = collider.transform.position;
                    DisiparDesdePunto(collisionPoint);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto colisionado está en la capa "capaNiebla"
        if (IsInFogLayer(collision.gameObject))
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            DisiparDesdePunto(collisionPoint);
        }
    }

    // Verifica si el objeto está en la capa de niebla
    bool IsInFogLayer(GameObject obj)
    {
        return ((1 << obj.layer) & capaNieblaLayer) != 0;
    }

    // Este método disipa la niebla desde un punto específico
    public void DisiparDesdePunto(Vector3 punto)
    {
        // Obtener el material actual del plano
        m_material = m_meshRenderer.material;

        // Calcula la distancia entre el punto de colisión y el centro del objeto
        float distance = Vector3.Distance(m_fogOfWarPlane.transform.position, punto);
        
        // Si la distancia es menor que el radio de disipación, reducimos la opacidad
        if (distance < m_radius)
        {
            float alpha = Mathf.Lerp(m_originalColor.a, 0f, distance / m_radius);  // Reducir la opacidad proporcionalmente
            m_material.color = new Color(m_originalColor.r, m_originalColor.g, m_originalColor.b, alpha);
        }
    }

    // Este método restaura la niebla a su estado original
    public void RestaurarNiebla()
    {
        if (m_material != null)
        {
            m_material.color = m_originalColor;  // Restauramos el color original con opacidad total
        }
    }

    // Inicializamos el MeshRenderer y el color original
    void Initialize()
    {
        m_meshRenderer = m_fogOfWarPlane.GetComponent<MeshRenderer>();
        if (m_meshRenderer != null)
        {
            m_material = m_meshRenderer.material;
            m_originalColor = m_material.color;  // Guardamos el color original del material
        }
        else
        {
            Debug.LogError("El plano no tiene un MeshRenderer.");
        }
    }
}
