using UnityEngine;

public class VisualManager: MonoBehaviour
{  
    Chunk[,] chunks; 
    public int chunksX = 0;
    public int chunksZ = 0;
    public int chunkSize = 0;

    [Range(1, 4)]
    public int resolution = 1;
    [Range(0, 20)]
    public float height = 1; 
    public float scale = 1;
    public int seed = 0;

    [Range(0, 1)]
    public float percistance = 0.5f;
    public float lacunarity = 5f;
    [Range(1, 5)]
    public int octaves = 1;


    public GameObject meshHandler;
    void Start()
    {
        //generates the chunks 
        chunks = new Chunk[chunksX, chunksZ];
        for (int x = 0; x < chunksX; x++) {
            for (int z = 0; z < chunksZ; z++) {
                GameObject MH = Instantiate(meshHandler, new Vector3(x*chunkSize, 0, z*chunkSize), Quaternion.identity);
                chunks[x, z] = MH.GetComponent<Chunk>();
            }
        }
        updateChunks();
    }

    private void Update()
    {
        //updateChunks();
    }

    //Updates the values for all the chunks. 
    void updateChunks() {
        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;
        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                chunks[x, z].offsetX = x * chunkSize;
                chunks[x, z].offsetZ = z * chunkSize;

                chunks[x, z].height = height;
                chunks[x, z].scale = scale;
                chunks[x, z].resolution = resolution;
                chunks[x, z].seed = seed;

                chunks[x, z].percistance = percistance;
                chunks[x, z].lacunarity = lacunarity;
                chunks[x, z].octaves = octaves;

                float chunkMaxHeight = chunks[x, z].getMaxHeight();
                float chunkMinHeight = chunks[x, z].getMinHeight();
                if (chunkMaxHeight > maxHeight)
                {
                    maxHeight = chunkMaxHeight;
                }
                if (chunkMinHeight < minHeight)
                {
                    minHeight = chunkMinHeight;
                }
            }
        }

        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                chunks[x, z].updateChunk(minHeight, maxHeight);
            }
        }
    }
}

