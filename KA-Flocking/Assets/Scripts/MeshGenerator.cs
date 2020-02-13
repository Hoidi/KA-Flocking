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
    private void Update()
    {
        if (Input.GetKey(KeyCode.U)){
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            CreateShape();
            updateMesh();
        }
    }

    void CreateShape()
    {
        int xNodes = xSize * resolution;
        int zNodes = zSize * resolution;
        float adjustedScale = scale / (float) resolution;

        vertices = new Vector3[(xNodes + 1) * (zNodes + 1)];
        Debug.Log((xNodes + 1) * (zNodes + 1));

        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        for (int z = 0, i = 0; z <= zNodes; z++)
        {
            for (int x = 0; x <= xNodes; x++)
            {
                if (lacunarity < 1)
                {
                    lacunarity = 1;
                }
                float amplitude = 1f;
                float frequency = 1f;
                float y = 0f;

                for(int j = 0; j< octaves; j++)
                {
                    float sampleX = x * frequency * adjustedScale;
                    float sampleZ = z * frequency * adjustedScale;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2f -1f;
                    y += perlinValue * amplitude;

                    amplitude *= percistance;
                    frequency *= lacunarity;
                }
                vertices[i] = new Vector3(x / (float)resolution, y, z / (float)resolution);
                if (y > maxValue)
                {
                    maxValue = y;
                } else if(y < minValue)
                {
                    minValue = y;
                }
                i++;
            }
        }

        //renomalizes the height values to be between 1 and -1. 
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = Mathf.InverseLerp(minValue, maxValue, vertices[i].y) * height;
        }

        triangles = new int[xNodes * zNodes * 6];


        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zNodes; z++) 
        { 
            for (int x = 0; x < xNodes; x++)
            {
                triangles[0 + tris] = vert + 0;
                triangles[1 + tris] = vert + xNodes + 1;
                triangles[2 + tris] = vert + 1;
                triangles[3 + tris] = vert + 1;
                triangles[4 + tris] = vert + xNodes + 1;
                triangles[5 + tris] = vert + xNodes + 2;

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
