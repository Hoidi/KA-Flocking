using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int resolution = 1;
    public int xSize = 20;
    public int zSize = 20;
    public int octaves = 1;
    public float height = 0.3f;
    public float scale = 0.3f;
    [Range(0, 1)]
    public float percistance = 0.5f;
    public float lacunarity = 5f;
    public int seed = 0;
    public Gradient gradient;

    public float colorMin;
    public float colorMax;


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
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        updateMesh();
    }

    //Creates a new mesh based on the public varables. 
    void CreateShape()
    {
        //Makes sure that octaves are not less than zero (otherwise it would break).
        if (octaves < 0)
        {
            octaves = 0;
        }
        //creates offsets for the octaves based on the seed. 
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        int xNodes = xSize * resolution;
        int zNodes = zSize * resolution;
        float adjustedScale = (float) 1 / (scale * resolution);

        vertices = new Vector3[(xNodes + 1) * (zNodes + 1)];
        Debug.Log((xNodes + 1) * (zNodes + 1));

        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;

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

                //here several layers of different sizes are merged to create more advanced behavior. 
                //octaves = number of layers. 
                //lacunarity = increase in frequency of the the layers (from mountains to rocks).
                //percistance = how fast the layers will decrease (a rock should not be the size of a mountain). 
                for (int j = 0; j < octaves; j++)
                {
                    float sampleX = x * frequency * adjustedScale + octaveOffsets[j].x;
                    float sampleZ = z * frequency * adjustedScale + octaveOffsets[j].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2f - 1f;
                    y += perlinValue * amplitude;

                    amplitude *= percistance;
                    frequency *= lacunarity;
                }
                vertices[i] = new Vector3((float) x / resolution, y, (float) z / resolution);
                if (y > maxHeight)
                {
                    maxHeight = y;
                } else if (y < minHeight)
                {
                    minHeight = y;
                }
                i++;
            }
        }

        //renomalizes the height values to be between 1 and -1 and multipiles with the wanted height. 
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y) * height;
        }

        triangles = new int[xNodes * zNodes * 6];



        //creates the triangles in the mesh
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

        //sets the colors of the mesh from a gradiant depending on the height.

        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zNodes; z++)
        {
            for (int x = 0; x <= xNodes; x++)
            {
                //float adjustedHeight = Mathf.InverseLerp(minHeight * height, maxHeight * height, vertices[i].y);
                float adjustedHeight = Mathf.InverseLerp(colorMin, colorMax,vertices[i].y);
                colors[i] = gradient.Evaluate(adjustedHeight);
                i++;
            }
        }
    }


    void updateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();


        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
