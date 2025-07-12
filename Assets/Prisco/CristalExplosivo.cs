using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CristalExplosivo : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float radioExplosion = 10f;
    public float dañoExplosion = 50f;
    public GameObject efectoExplosion;
    public LayerMask capaEnemigos = 1 << 6; // Ajustar según la capa de enemigos
    
    [Header("Crystal Settings")]
    public float cooldownTiempo = 5f;
    private float ultimaActivacion = -5f; // Permitir uso inmediato al inicio
    private bool enCooldown = false;
    
    [Header("Visual Settings")]
    public Material materialDañado;
    public Material materialCooldown;
    private Material materialOriginal;
    private Renderer cristalRenderer;
    
    [Header("Area Visualization")]
    public bool mostrarAreaExplosion = true;
    public GameObject areaVisualizationPrefab; // Prefab para mostrar el área
    private GameObject areaVisualizationInstance;
    public Color areaColor = new Color(1f, 0f, 0f, 0.3f); // Rojo transparente
    public Color areaColorCooldown = new Color(0.5f, 0.5f, 0.5f, 0.2f); // Gris transparente
    public float duracionVisualizacionArea = 2f; // Duración en segundos que se muestra el área
    private Coroutine coroutineVisualizacionArea;
    
    [Header("Chain Reaction")]
    public bool activarCadena = true;
    public float delayActivacionCadena = 0.2f; // Delay entre activaciones en cadena
    
    void Start()
    {
        cristalRenderer = GetComponent<Renderer>();
        if (cristalRenderer != null)
        {
            materialOriginal = cristalRenderer.material;
        }
        
        // Asegurar que el cristal tenga un Trigger Collider para detectar ataques
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        
        // Crear visualización del área de explosión (inicialmente oculta)
        CrearVisualizacionArea();
        if (areaVisualizationInstance != null)
        {
            areaVisualizationInstance.SetActive(false); // Ocultar inicialmente
        }
    }
    
    void Update()
    {
        // Verificar si el cooldown ha terminado
        if (enCooldown && Time.time >= ultimaActivacion + cooldownTiempo)
        {
            enCooldown = false;
            RestaurarMaterialOriginal();
            ActualizarVisualizacionArea();
            Debug.Log("Cristal explosivo listo para usar nuevamente");
        }
        
        // Actualizar el color del área según el estado
        ActualizarVisualizacionArea();
    }
    
    private void CrearVisualizacionArea()
    {
        if (!mostrarAreaExplosion) return;
        
        // Si no hay prefab asignado, crear uno proceduralmente
        if (areaVisualizationPrefab == null)
        {
            CrearVisualizacionProcedural();
        }
        else
        {
            // Usar el prefab asignado
            areaVisualizationInstance = Instantiate(areaVisualizationPrefab, transform.position, Quaternion.identity, transform);
            areaVisualizationInstance.transform.localScale = Vector3.one * radioExplosion * 2;
        }
    }
    
    private void CrearVisualizacionProcedural()
    {
        // Crear un GameObject hijo para la visualización
        areaVisualizationInstance = new GameObject("AreaVisualization");
        areaVisualizationInstance.transform.SetParent(transform);
        areaVisualizationInstance.transform.localPosition = Vector3.zero;
        
        // Agregar MeshRenderer y MeshFilter
        MeshRenderer renderer = areaVisualizationInstance.AddComponent<MeshRenderer>();
        MeshFilter filter = areaVisualizationInstance.AddComponent<MeshFilter>();
        
        // Crear un cilindro plano para mostrar el área
        filter.mesh = CreateCircleMesh();
        
        // Crear material transparente
        Material areaMaterial = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        areaMaterial.color = areaColor;
        renderer.material = areaMaterial;
        
        // Escalar para que coincida con el radio de explosión
        areaVisualizationInstance.transform.localScale = Vector3.one * radioExplosion * 2;
        areaVisualizationInstance.transform.localRotation = Quaternion.Euler(90, 0, 0);
    }
    
    private Mesh CreateCircleMesh()
    {
        Mesh mesh = new Mesh();
        int segments = 32;
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];
        
        // Centro del círculo
        vertices[0] = Vector3.zero;
        
        // Vértices del perímetro
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle) * 0.5f, 0, Mathf.Sin(angle) * 0.5f);
        }
        
        // Triángulos
        for (int i = 0; i < segments; i++)
        {
            int triangleIndex = i * 3;
            triangles[triangleIndex] = 0;
            triangles[triangleIndex + 1] = i + 1;
            triangles[triangleIndex + 2] = (i + 1) % segments + 1;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    private void ActualizarVisualizacionArea()
    {
        if (areaVisualizationInstance == null) return;
        
        MeshRenderer renderer = areaVisualizationInstance.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Color targetColor = enCooldown ? areaColorCooldown : areaColor;
            renderer.material.color = targetColor;
        }
    }
    
    // Detectar colisión solo con ataques específicos del jugador
    private void OnTriggerEnter(Collider other)
    {
        if (enCooldown) return;
        
        // Solo activar con ataques específicos del jugador
        if (EsAtaqueDelJugador(other))
        {
            Debug.Log($"Cristal activado por: {other.name}");
            ActivarExplosion();
        }
    }
    
    private bool EsAtaqueDelJugador(Collider other)
    {
        // NO activar con el jugador directamente, solo con sus ataques
        if (other.CompareTag("Player"))
            return false;
            
        bool esAtaque = false;
            
        // Verificar si es un proyectil del jugador
        Boomerang boomerang = other.GetComponent<Boomerang>();
        if (boomerang != null)
            esAtaque = true;
            
        DisparoCargado disparoCargado = other.GetComponent<DisparoCargado>();
        if (disparoCargado != null)
            esAtaque = true;
            
        // Verificar si tiene DamageDealer (ataques cuerpo a cuerpo)
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();
        if (damageDealer != null)
            esAtaque = true;
            
        // Verificar si el objeto padre tiene el tag Player (para ataques melee)
        if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
            esAtaque = true;
        
        // Si es un ataque, mostrar el área de explosión temporalmente
        if (esAtaque && mostrarAreaExplosion)
        {
            MostrarAreaExplosionTemporal();
        }
            
        return esAtaque;
    }
    
    // Método público para activar manualmente si es necesario
    public void ActivarExplosion()
    {
        if (enCooldown) 
        {
            Debug.Log("Cristal en cooldown, no se puede activar");
            return;
        }
        
        ultimaActivacion = Time.time;
        enCooldown = true;
        
        Debug.Log("¡Cristal explosivo activado!");
        
        // Cambiar inmediatamente al material de activación/daño
        CambiarMaterial(materialDañado);
        
        Explotar();
    }
    
    // Método para activación en cadena (sin logs excesivos)
    public void ActivarExplosionEnCadena()
    {
        if (enCooldown) return;
        
        ultimaActivacion = Time.time;
        enCooldown = true;
        
        // Cambiar inmediatamente al material de activación/daño
        CambiarMaterial(materialDañado);
        
        // Delay para activación en cadena
        StartCoroutine(ExplotarConDelay());
    }
    
    private IEnumerator ExplotarConDelay()
    {
        yield return new WaitForSeconds(delayActivacionCadena);
        Explotar();
    }
    
    private void Explotar()
    {
        Debug.Log("¡Cristal explosivo detonado!");
        
        // Crear efecto visual de explosión
        if (efectoExplosion != null)
        {
            Instantiate(efectoExplosion, transform.position, Quaternion.identity);
        }
        
        // Detectar enemigos en el área de explosión usando múltiples métodos
        List<GameObject> enemigosAfectados = new List<GameObject>();
        
        // Método 1: OverlapSphere con LayerMask
        Collider[] enemigosEnArea = Physics.OverlapSphere(transform.position, radioExplosion, capaEnemigos);
        
        foreach (Collider enemigo in enemigosEnArea)
        {
            if (!enemigosAfectados.Contains(enemigo.gameObject))
            {
                enemigosAfectados.Add(enemigo.gameObject);
            }
        }
        
        // Método 2: Buscar por tag si el LayerMask no funciona correctamente
        GameObject[] enemigosConTag = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemigo in enemigosConTag)
        {
            float distancia = Vector3.Distance(transform.position, enemigo.transform.position);
            if (distancia <= radioExplosion && !enemigosAfectados.Contains(enemigo))
            {
                enemigosAfectados.Add(enemigo);
            }
        }
        
        // Detectar otros cristales explosivos en el área para activación en cadena
        if (activarCadena)
        {
            DetectarYActivarCristalesCercanos();
        }
        
        // Aplicar daño y desactivar revivir a todos los enemigos afectados
        foreach (GameObject enemigo in enemigosAfectados)
        {
            // Intentar aplicar daño con Life (sistema principal)
            Life vidaEnemigo = enemigo.GetComponent<Life>();
            if (vidaEnemigo != null)
            {
                vidaEnemigo.ModifyTime(-dañoExplosion);
                Debug.Log($"Enemigo {enemigo.name} recibió {dañoExplosion} de daño por explosión (Life)");
            }
            else
            {
                // Intentar aplicar daño con EnemyLife (sistema alternativo)
                EnemyLife enemyLife = enemigo.GetComponent<EnemyLife>();
                if (enemyLife != null)
                {
                    enemyLife.ReceiveDamage(dañoExplosion);
                    Debug.Log($"Enemigo {enemigo.name} recibió {dañoExplosion} de daño por explosión (EnemyLife)");
                }
            }
            
            // Desactivar capacidad de revivir
            DesactivarRevivir(enemigo);
        }
        
        Debug.Log($"Cristal explosivo afectó a {enemigosAfectados.Count} enemigos");
        
        // Iniciar cooldown visual
        StartCoroutine(MostrarCooldown());
    }
    
    private void DetectarYActivarCristalesCercanos()
    {
        // Buscar todos los cristales explosivos en el área
        Collider[] objetosEnArea = Physics.OverlapSphere(transform.position, radioExplosion);
        
        foreach (Collider objeto in objetosEnArea)
        {
            CristalExplosivo otroCristal = objeto.GetComponent<CristalExplosivo>();
            
            // Si es otro cristal explosivo y no está en cooldown
            if (otroCristal != null && otroCristal != this && !otroCristal.enCooldown)
            {
                Debug.Log($"Activando cristal en cadena: {otroCristal.name}");
                otroCristal.ActivarExplosionEnCadena();
            }
        }
    }
    
    private IEnumerator MostrarCooldown()
    {
        // Esperar un breve momento para mostrar el material de activación
        yield return new WaitForSeconds(0.3f);
        
        // Cambiar al material de cooldown para el resto del tiempo
        CambiarMaterial(materialCooldown);
        
        // Ocultar el área de visualización al cambiar al material de cooldown
        if (areaVisualizationInstance != null)
        {
            areaVisualizationInstance.SetActive(false);
            Debug.Log("Área de visualización ocultada al entrar en cooldown");
        }
        
        // Detener la corrutina de visualización temporal si está activa
        if (coroutineVisualizacionArea != null)
        {
            StopCoroutine(coroutineVisualizacionArea);
            coroutineVisualizacionArea = null;
        }
        
        // Esperar el resto del tiempo de cooldown
        yield return new WaitForSeconds(cooldownTiempo - 0.3f);
        
        // El material se restaurará automáticamente en Update()
    }
    
    private void RestaurarMaterialOriginal()
    {
        CambiarMaterial(materialOriginal);
    }
    
    // Nuevo método centralizado para cambiar materiales
    private void CambiarMaterial(Material nuevoMaterial)
    {
        if (cristalRenderer != null && nuevoMaterial != null)
        {
            cristalRenderer.material = nuevoMaterial;
        }
    }
    
    private void DesactivarRevivir(GameObject enemigo)
    {
        bool revivirDesactivado = false;
        
        // Buscar y desactivar en EnemyLife
        EnemyLife enemyLife = enemigo.GetComponent<EnemyLife>();
        if (enemyLife != null)
        {
            // Usar reflexión para encontrar el campo revivir
            var campo = typeof(EnemyLife).GetField("revivir");
            if (campo != null && campo.FieldType == typeof(bool))
            {
                campo.SetValue(enemyLife, false);
                Debug.Log($"Desactivada capacidad de revivir en {enemigo.name}: revivir = false");
                revivirDesactivado = true;
            }
            
            // También activar antiRevivir si existe
            var campoAnti = typeof(EnemyLife).GetField("antiRevivir");
            if (campoAnti != null && campoAnti.FieldType == typeof(bool))
            {
                campoAnti.SetValue(enemyLife, true);
                Debug.Log($"Activado antiRevivir en {enemigo.name}: antiRevivir = true");
            }
        }
        
        // Buscar en otros scripts que puedan manejar revivir
        MonoBehaviour[] scripts = enemigo.GetComponents<MonoBehaviour>();
        
        foreach (MonoBehaviour script in scripts)
        {
            var tipo = script.GetType();
            var campos = tipo.GetFields();
            
            foreach (var campo in campos)
            {
                // Buscar campos booleanos que contengan "reviv" en el nombre
                if (campo.FieldType == typeof(bool) && 
                    (campo.Name.ToLower().Contains("reviv") || 
                     campo.Name.ToLower().Contains("resurrection") ||
                     campo.Name.ToLower().Contains("canrevive")))
                {
                    // Si es un campo de "antiRevivir", activarlo; si es "revivir", desactivarlo
                    if (campo.Name.ToLower().Contains("anti"))
                    {
                        campo.SetValue(script, true);
                        Debug.Log($"Activado {campo.Name} en {enemigo.name}: {campo.Name} = true");
                    }
                    else
                    {
                        campo.SetValue(script, false);
                        Debug.Log($"Desactivado {campo.Name} en {enemigo.name}: {campo.Name} = false");
                    }
                    revivirDesactivado = true;
                }
            }
            
            // También buscar propiedades
            var propiedades = tipo.GetProperties();
            foreach (var propiedad in propiedades)
            {
                if (propiedad.PropertyType == typeof(bool) && propiedad.CanWrite &&
                    (propiedad.Name.ToLower().Contains("reviv") || 
                     propiedad.Name.ToLower().Contains("resurrection") ||
                     propiedad.Name.ToLower().Contains("canrevive")))
                {
                    if (propiedad.Name.ToLower().Contains("anti"))
                    {
                        propiedad.SetValue(script, true);
                        Debug.Log($"Activado {propiedad.Name} en {enemigo.name}: {propiedad.Name} = true");
                    }
                    else
                    {
                        propiedad.SetValue(script, false);
                        Debug.Log($"Desactivado {propiedad.Name} en {enemigo.name}: {propiedad.Name} = false");
                    }
                    revivirDesactivado = true;
                }
            }
        }
        
        if (!revivirDesactivado)
        {
            Debug.LogWarning($"No se encontraron variables de revivir en {enemigo.name}");
        }
    }
    
    // Visualizar el radio de explosión en el editor
    private void OnDrawGizmosSelected()
    {
        // Color del gizmo según el estado
        if (enCooldown)
        {
            // Durante cooldown: gris
            Gizmos.color = Color.gray;
        }
        else
        {
            // Listo para usar: rojo
            Gizmos.color = Color.red;
        }
        
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
        
        // Mostrar información adicional del estado
        if (enCooldown)
        {
            Gizmos.color = Color.yellow;
            float tiempoRestante = (ultimaActivacion + cooldownTiempo) - Time.time;
            
            // Mostrar cubo indicador de cooldown
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2, Vector3.one * 0.5f);
            
            // Mostrar progreso de cooldown con línea
            float progreso = 1f - (tiempoRestante / cooldownTiempo);
            Vector3 inicio = transform.position + Vector3.up * 3;
            Vector3 fin = inicio + Vector3.right * 2f * progreso;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(inicio, fin);
        }
        
        // Mostrar otros cristales en rango para efecto en cadena
        if (activarCadena)
        {
            Gizmos.color = Color.cyan;
            Collider[] objetosEnArea = Physics.OverlapSphere(transform.position, radioExplosion);
            
            foreach (Collider objeto in objetosEnArea)
            {
                CristalExplosivo otroCristal = objeto.GetComponent<CristalExplosivo>();
                if (otroCristal != null && otroCristal != this)
                {
                    // Línea diferente según si el otro cristal está disponible
                    if (otroCristal.enCooldown)
                    {
                        Gizmos.color = Color.gray;
                    }
                    else
                    {
                        Gizmos.color = Color.cyan;
                    }
                    
                    Gizmos.DrawLine(transform.position, otroCristal.transform.position);
                    Gizmos.DrawWireCube(otroCristal.transform.position, Vector3.one * 0.5f);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Mostrar el área de explosión siempre visible
        if (mostrarAreaExplosion)
        {
            // Color según el estado
            if (enCooldown)
            {
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.1f); // Gris transparente
            }
            else
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.1f); // Rojo transparente
            }
            
            Gizmos.DrawSphere(transform.position, radioExplosion);
        }
    }
    
    // Método para verificar si está en cooldown (útil para UI o otros scripts)
    public bool EstaEnCooldown()
    {
        return enCooldown;
    }
    
    // Método para obtener el tiempo restante de cooldown
    public float TiempoRestanteCooldown()
    {
        if (!enCooldown) return 0f;
        return Mathf.Max(0f, (ultimaActivacion + cooldownTiempo) - Time.time);
    }
    
    private void OnDestroy()
    {
        // Limpiar la visualización al destruir el objeto
        if (areaVisualizationInstance != null)
        {
            DestroyImmediate(areaVisualizationInstance);
        }
        
        // Detener corrutinas activas
        if (coroutineVisualizacionArea != null)
        {
            StopCoroutine(coroutineVisualizacionArea);
        }
    }
    
    private void MostrarAreaExplosionTemporal()
    {
        Debug.Log("Cristal recibió daño - mostrando área de explosión");
        
        // Si ya hay una visualización activa, cancelarla
        if (coroutineVisualizacionArea != null)
        {
            StopCoroutine(coroutineVisualizacionArea);
        }
        
        // Mostrar el área de explosión
        if (areaVisualizationInstance != null)
        {
            areaVisualizationInstance.SetActive(true);
            Debug.Log("Área de explosión activada y visible");
        }
        
        // Iniciar corrutina para ocultar después del tiempo especificado
        coroutineVisualizacionArea = StartCoroutine(OcultarAreaExplosionDespuesDeDelay());
    }
    
    private IEnumerator OcultarAreaExplosionDespuesDeDelay()
    {
        Debug.Log($"Iniciando timer para ocultar área de explosión en {duracionVisualizacionArea} segundos");
        yield return new WaitForSeconds(duracionVisualizacionArea);
        
        if (areaVisualizationInstance != null)
        {
            areaVisualizationInstance.SetActive(false);
            Debug.Log("Área de explosión ocultada");
        }
        
        coroutineVisualizacionArea = null;
    }
}

