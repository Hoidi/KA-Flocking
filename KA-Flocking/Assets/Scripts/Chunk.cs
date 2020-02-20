using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]

//The chunk class hold the mesh for a chunk. 
//Large portions of the code was created thorugh following the turtorials of the two youtubers Sebastian Lague and Brackeys. 
public class Chunk : MonoBehaviour
{
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    //for recording the min and max hight.
    private float maxHeight = float.MinValue;
    private float minHeight = float.MaxValue;

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
    public float offsetX = 0;
    public float offsetZ = 0; 
    public Gradient gradient;

    public float colorMin;
    public float colorMax;


    // Start is called before the first frame update
    void Start()
    {
        updateChunk(minHeight, maxHeight);
    }

    //Creates a new mesh based on the public varables. 
    void CreateShape()
    {
        //Makes sure that values are not out of bounds.
        if (octaves < 0)
        {
            octaves = 0;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        //creates offsets for the octaves based on the seed. 
        System.Random prng = new System.Random(seed);
        float[] octaveOffsetsX = new float[octaves];
        float[] octaveOffsetsZ = new float[octaves];


        for (int i = 0; i < octaves; i++)
        {
            octaveOffsetsX[i] = prng.Next(-100000, 100000);
            octaveOffsetsZ[i] = prng.Next(-100000, 100000);
        }

        //ensures that the map size remains constant for all resolutions.
        int adjustedXSize = xSize * resolution;
        int adjustedZSize = zSize * resolution;
        float adjstedOffsetX = offsetX * resolution;
        float adjustedOffsetZ = offsetZ * resolution; 
        float adjustedScale = (float) 1 / (scale * resolution);

        //here is where all the positional data will be stored. 
        vertices = new Vector3[(adjustedXSize + 1) * (adjustedZSize + 1)];

        for (int z = 0, i = 0; z <= adjustedZSize; z++)
        {
            for (int x = 0; x <= adjustedXSize; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float y = 0f;

                //here several layers of different sizes are merged to create more advanced behavior. 
                //octaves = number of layers. 
                //lacunarity = increase in frequency of the the layers (from mountains to rocks).
                //percistance = how fast the layers will decrease (a rock should not be the size of a mountain). 
                for (int j = 0; j < octaves; j++)
                {
                    float sampleX = ((x + adjstedOffsetX) * frequency * adjustedScale + octaveOffsetsX[j]);
                    float sampleZ = ((z + adjustedOffsetZ) * frequency * adjustedScale + octaveOffsetsZ[j]);

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

        triangles = new int[adjustedXSize * adjustedZSize * 6];

        //creates the triangles in the mesh
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < adjustedZSize; z++)
        {
            for (int x = 0; x < adjustedXSize; x++)
            {
                triangles[0 + tris] = vert + 0;
                triangles[1 + tris] = vert + adjustedXSize + 1;
                triangles[2 + tris] = vert + 1;
                triangles[3 + tris] = vert + 1;
                triangles[4 + tris] = vert + adjustedXSize + 1;
                triangles[5 + tris] = vert + adjustedXSize + 2;

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
        mesh.colors = colors;

        //makes sure that lighting is good.
        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void renormaliseHeight(float minHeight, float maxHeight)
    {
        //renomalizes the height values to be between 1 and -1 and multipiles with the wanted height. 
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y) * height;
        }
    }

    void updateColors()
    {
        //sets the colors of the mesh from a gradiant depending on the height.
        colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            //float adjustedHeight = Mathf.InverseLerp(minHeight * height, maxHeight * height, vertices[i].y);
            float adjustedHeight = Mathf.InverseLerp(colorMin, colorMax, vertices[i].y);
            colors[i] = gradient.Evaluate(adjustedHeight);
        }
    }
    public void updateChunk(float minHeight, float maxHeight)
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        renormaliseHeight(minHeight, maxHeight);
        updateColors();
        updateMesh();
    }

    public float getMaxHeight()
    {
        return maxHeight;
    }

    public float getMinHeight()
    {
        return minHeight;
    }
}
