using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]

//The chunk class hold the mesh for a chunk. 
//Large portions of the code was created thorugh following the turtorials of the two youtubers Sebastian Lague and Brackeys. 
public abstract class Chunk : MonoBehaviour
{
    protected Mesh mesh;

    protected Vector3[] vertices;
    protected int[] triangles;

    //for recording the min and max hight.
    protected float localMaxHeight = float.MinValue;
    protected float localMinHeight = float.MaxValue;

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

    //updated the colors
    protected abstract void updateColors();

    //Creates a new mesh based on the public varables. 
    protected void CreateShape()
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
        float adjustedScale = (float)1 / (scale * resolution);

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
                vertices[i] = new Vector3((float)x / resolution, y, (float)z / resolution);
                if (y > localMaxHeight)
                {
                    localMaxHeight = y;
                }
                else if (y < localMinHeight)
                {
                    localMinHeight = y;
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

    protected void renormaliseHeight(float minHeight, float maxHeight)
    {
        //renomalizes the height values to be between 1 and -1 and multipiles with the wanted height. 
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y) * height;
        }
    }
    public void updateChunk(float minHeight, float maxHeight)
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        CreateShape();
        renormaliseHeight(minHeight, maxHeight);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        updateColors();
        mesh.RecalculateNormals();  //makes sure that lighting is good.
        GetComponent<MeshCollider>().sharedMesh = mesh;  // updates the mesh in Unity.
    }

    public float getMaxHeight()
    {
        return localMaxHeight;
    }

    public float getMinHeight()
    {
        return localMinHeight;
    }
}
