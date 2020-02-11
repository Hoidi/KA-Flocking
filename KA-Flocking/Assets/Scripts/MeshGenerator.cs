using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int resolution = 1;
    public int xSize = 20;
    public int zSize = 20;
    public int octaves = 1;
    public float height = 0.3f;
    public float scale = 0.3f;
    [Range(0, 1)]
    public float percistance = 0.5f;
    public float lacunarity = 5f;
    public 

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        updateMesh();
    }

    void CreateShape()
    {
        xSize *= resolution;
        zSize *= resolution;
        scale /= resolution;

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Debug.Log((xSize + 1) * (zSize + 1));

        for (int z = 0, i = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                if (lacunarity < 1)
                {
                    lacunarity = 1;
                }
                float amplitude = 1f;
                float frequency = 1f;
                float noiceHeight = 0f;
                float y = 0;
                for(int j = 0; j< octaves; j++)
                {
                    float sampleX = x * frequency * scale;
                    float sampleZ = z * frequency * scale;

                    y = Mathf.PerlinNoise(sampleX, sampleZ) * 2f -1f;
                    noiceHeight += y * amplitude;

                    amplitude *= percistance;
                    frequency *= lacunarity;
                }
                vertices[i] = new Vector3(x / (float)resolution, y * height, z / (float)resolution);
                i++;
            }

        }

        triangles = new int[xSize * zSize * 6];


        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++) 
        { 
            for (int x = 0; x < xSize; x++)
            {
                triangles[0 + tris] = vert + 0;
                triangles[1 + tris] = vert + xSize + 1;
                triangles[2 + tris] = vert + 1;
                triangles[3 + tris] = vert + 1;
                triangles[4 + tris] = vert + xSize + 1;
                triangles[5 + tris] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }  
    }


    void updateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();


        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
