using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrain : MonoBehaviour
{
    Mesh terrainMesh;

    Vector3[] vertices;
    int[] triangles;

    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;

    public Gradient gradient;

    public float minTerrainHeight;
    public float maxTerrainHeight;

    [Range(0f,0.5f)]
    public float heightFactor;

    [Range(0,10)]
    public int offsetX;
    [Range(0, 10)]
    public int offsetZ;


    // Start is called before the first frame update
    void Start()
    {
        terrainMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = terrainMesh;        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMesh();
    }

    public void CreateMesh() {
        StartCoroutine(GenerateMesh(heightFactor));
    }

    IEnumerator GenerateMesh(float hei) {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z=0 ; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                float y = Mathf.PerlinNoise(x * hei + offsetX, z * hei + offsetZ) * 5f;
                vertices[i] = new Vector3(x, y, z);

                if (y < minTerrainHeight)
                    minTerrainHeight = y;
                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;

                i++;
            }
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);

                i++;
            }
        }
        
        triangles = new int[xSize * zSize * 6];

        int vertex = 0;
        int tris = 0;

        for (int k = 0; k < zSize; k++) {
            for (int i = 0; i < xSize; i++)
            {
                triangles[tris + 0] = vertex + 0;
                triangles[tris + 1] = vertex + xSize + 1;
                triangles[tris + 2] = vertex + 1;
                triangles[tris + 3] = vertex + 1;
                triangles[tris + 4] = vertex + xSize + 1;
                triangles[tris + 5] = vertex + xSize + 2;

                vertex++;
                tris += 6;

                yield return new WaitForSeconds(0.000000000000000000001f);
                
            }
            vertex++;   
        }

        
    }

    void UpdateMesh() {
        terrainMesh.Clear();

        terrainMesh.vertices = vertices;
        terrainMesh.triangles = triangles;
        terrainMesh.colors = colors;

        terrainMesh.RecalculateNormals();
    }

}
