using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    public List<GameObject> fogOfWarPlanes; // Cambiado a una lista de objetos
    public LayerMask fogLayer;

    private List<Mesh> meshes = new List<Mesh>();
    private List<Vector3[]> verticesList = new List<Vector3[]>();
    private List<Color[]> colorsList = new List<Color[]>();

    public List<ObjDNiebla> nieblaSources = new List<ObjDNiebla>();

    void Start()
    {
        // Buscar objetos con el material "Niebla" y agregarlos a la lista
        foreach (var renderer in FindObjectsOfType<Renderer>())
        {
            if (renderer.sharedMaterial != null && renderer.sharedMaterial.name == "Niebla")
            {
                fogOfWarPlanes.Add(renderer.gameObject);
            }
        }

        foreach (var fogPlane in fogOfWarPlanes)
        {
            var mesh = fogPlane.GetComponent<MeshFilter>().mesh;
            meshes.Add(mesh);
            verticesList.Add(mesh.vertices);
            colorsList.Add(new Color[mesh.vertices.Length]);
        }

        ResetColors();
        UpdateMeshColors();
    }

    void Update()
    {
        ResetColors();

        foreach (var obj in nieblaSources)
        {
            if (obj != null && obj.AreaActiva())
            {
                ApplyNieblaInfluence(obj);
            }
        }

        UpdateMeshColors();
    }

    void ResetColors()
    {
        for (int j = 0; j < colorsList.Count; j++)
        {
            for (int i = 0; i < colorsList[j].Length; i++)
            {
                colorsList[j][i] = Color.black; // Opaco por defecto
            }
        }
    }

    void ApplyNieblaInfluence(ObjDNiebla obj)
    {
        if (!(obj.areaDeNiebla is SphereCollider sphere)) return;

        float scaledRadius = sphere.radius * Mathf.Max(
            sphere.transform.lossyScale.x,
            sphere.transform.lossyScale.y,
            sphere.transform.lossyScale.z
        );

        float innerRadius = scaledRadius * 0.8f;
        float radiusSqr = scaledRadius * scaledRadius;
        float innerRadiusSqr = innerRadius * innerRadius;

        Vector3 worldPos = sphere.transform.position;

        for (int j = 0; j < verticesList.Count; j++)
        {
            var vertices = verticesList[j];
            var colors = colorsList[j];
            var fogPlane = fogOfWarPlanes[j];

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertexWorld = fogPlane.transform.TransformPoint(vertices[i]);
                float distSqr = (vertexWorld - worldPos).sqrMagnitude;

                if (distSqr < radiusSqr)
                {
                    if (distSqr < innerRadiusSqr)
                    {
                        colors[i].a = 0f;
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01((distSqr - innerRadiusSqr) / (radiusSqr - innerRadiusSqr));
                        colors[i].a = Mathf.Min(colors[i].a, alpha);
                    }
                }
            }
        }
    }

    void UpdateMeshColors()
    {
        for (int j = 0; j < meshes.Count; j++)
        {
            meshes[j].colors = colorsList[j];
        }
    }
}
