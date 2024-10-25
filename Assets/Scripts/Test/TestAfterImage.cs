using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAfterImage : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] _skinnedMeshRenderers;
    [SerializeField] private Material customMaterial;
    [SerializeField] private float creationInterval = 0.5f;
    [SerializeField] private float imageLifetime = 1.5f;
    [SerializeField] private float fadeDuration = 1f;

    private PlayerMovement playerMovement; // Referencia al script de PlayerMovement

    void Start()
    {
        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        playerMovement = GetComponent<PlayerMovement>(); // Obtener el componente PlayerMovement en el mismo objeto
        StartCoroutine(CreateAfterImage());
    }

    IEnumerator CreateAfterImage()
    {
        while (true)
        {
            // Esperar si el script está desactivado o si el bullet time no está activo
            if (!enabled || playerMovement == null || !playerMovement.bulletTime) 
            {
                yield return null;
            }
            else
            {
                for (int j = 0; j < _skinnedMeshRenderers.Length; j++)
                {
                    GameObject obj = new GameObject();
                    obj.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
                    MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
                    MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
                    Mesh mesh = new Mesh();
                    _skinnedMeshRenderers[j].BakeMesh(mesh);
                    meshFilter.mesh = mesh;

                    Material materialToUse = customMaterial != null ? customMaterial : _skinnedMeshRenderers[j].material;
                    meshRenderer.material = new Material(materialToUse);

                    StartCoroutine(FadeOutAndDestroy(obj, meshRenderer.material, imageLifetime, fadeDuration));
                }
            }

            yield return new WaitForSeconds(creationInterval);
        }
    }

    IEnumerator FadeOutAndDestroy(GameObject obj, Material material, float lifetime, float fadeDuration)
    {
        yield return new WaitForSeconds(lifetime - fadeDuration);

        float elapsedTime = 0f;
        Color initialColor = material.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            material.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        Destroy(obj);
    }
}