using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMallaManager : MonoBehaviour
{
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = new List<Vector3>(mesh.vertices);
        triangles = new List<int>(mesh.triangles);
    }

    public void EliminarHexagonosDentroDelRadio(Vector3 centro, float radio)
    {
        // Por simplicidad, vamos a quitar todos los triángulos cuyo centro esté dentro del radio
        List<int> nuevosTriangulos = new List<int>();

        for (int i = 0; i < triangles.Count; i += 3)
        {
            Vector3 v1 = vertices[triangles[i]];
            Vector3 v2 = vertices[triangles[i + 1]];
            Vector3 v3 = vertices[triangles[i + 2]];

            Vector3 centroTriangulo = (v1 + v2 + v3) / 3f;

            if (Vector3.Distance(centroTriangulo, centro) > radio)
            {
                nuevosTriangulos.Add(triangles[i]);
                nuevosTriangulos.Add(triangles[i + 1]);
                nuevosTriangulos.Add(triangles[i + 2]);
            }
        }

        mesh.triangles = nuevosTriangulos.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}

