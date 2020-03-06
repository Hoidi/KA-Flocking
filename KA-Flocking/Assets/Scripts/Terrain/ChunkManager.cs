﻿using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    Chunk[,] chunks;
    public int chunksX = 0;
    public int chunksZ = 0;
    public int chunkSize = 0;

    [Range(1, 4)]
    public int resolution = 1;
    [Range(0, 20)]
    public float height = 1;
    [Range(0, 2)]
    public float pointyness = 1;
    public float pointyBreakOff = 0;
    public float scale = 1;
    public int seed = 0;

    [Range(0, 1)]
    public float percistance = 0.5f;
    public float lacunarity = 5f;
    [Range(1, 5)]
    public int octaves = 1;
    public GameObject wallPrefab;


    public GameObject chunkType;
    void Start()
    {
        BuildWall(new Vector3(-312,0,-312), Quaternion.identity);
        BuildWall(new Vector3(-312,0,312), Quaternion.identity);
        BuildWall(new Vector3(-312,0,-312), new Quaternion (0,0.7071f,0,0.7071f));
        BuildWall(new Vector3(312,0,-312), new Quaternion (0,0.7071f,0,0.7071f));
        //generates the chunks 
        seed = Random.Range(0, 10000);
        chunks = new Chunk[chunksX, chunksZ];
        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                GameObject MH = Instantiate(chunkType, new Vector3(x * chunkSize - (chunksX * chunkSize)/2, 0, z * chunkSize - (chunksZ * chunkSize)/2), Quaternion.identity, transform);
                chunks[x, z] = MH.GetComponent<Chunk>();
            }
        }
        updateChunks();
        DontDestroyOnLoad(this.gameObject);
    }

    private void BuildWall (Vector3 location, Quaternion rotation) {
        Vector3 direction;
        if (rotation == Quaternion.identity) {
            direction = Vector3.right;
        } else {
            direction = Vector3.forward;
        }
        for (int i = 0; i < 10; i++)
        {
            // Each wall has a size of around 62.5 after scaling
            GameObject go = Instantiate(wallPrefab, location + direction*i*62.5f + direction*31.25f, rotation,transform);
            go.transform.localScale = new Vector3(12.5f,12.5f,1.0f);

        }
    }

    private void Update()
    {
        // enable for to be able to change the level during runtime. (is very laggy with bigger levels)
        //updateChunks();
    }

    //Updates the values for all the chunks. 
    void updateChunks()
    {
        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;
        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                chunks[x, z].offsetX = x * chunkSize;
                chunks[x, z].offsetZ = z * chunkSize;

                chunks[x, z].xSize = chunkSize;
                chunks[x, z].zSize = chunkSize;

                chunks[x, z].height = height;
                chunks[x, z].pointyness = pointyness;
                chunks[x, z].pointyBreakOff = pointyBreakOff;
                chunks[x, z].scale = scale;
                chunks[x, z].resolution = resolution;
                chunks[x, z].seed = seed;

                chunks[x, z].percistance = percistance;
                chunks[x, z].lacunarity = lacunarity;
                chunks[x, z].octaves = octaves;

                chunks[x, z].updateChunk(-1, 1); // to make them have max and min values. 

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

