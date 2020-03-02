using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartoonChunk : Chunk
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer; 
    protected Vector2[] uvs;
    protected override void updateColors()
    {
        int adjustedXSize = xSize * resolution;
        int adjustedZSize = zSize * resolution;
        float[,] heightMap = new float[adjustedXSize + 1, adjustedZSize + 1];
        uvs = new Vector2[(adjustedXSize + 1) * (adjustedZSize + 1)];

        int vertexIndex = 0;
        for (int x = 0; x <= adjustedXSize; x++)
        {
            for (int z = 0; z <= adjustedZSize; z++)
            {
                uvs[vertexIndex] = new Vector2(x / (float)adjustedXSize, z / (float)adjustedZSize);
                heightMap[x, z] = Mathf.InverseLerp(0, 10, vertices[vertexIndex].y);
                vertexIndex++;
            }
        }

        mesh.uv = uvs;
        meshRenderer.sharedMaterial = new Material(meshRenderer.sharedMaterial);
        meshRenderer.sharedMaterial.mainTexture = TextureGenerator.TextureFromHeightMap(heightMap);
    }
}
