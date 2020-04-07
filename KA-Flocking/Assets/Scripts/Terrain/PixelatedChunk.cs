using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a chunk that creates a texture instead of vertice colors. 
public class PixelatedChunk : Chunk
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer; 
    protected Vector2[] uvs;
    protected override void updateColors(int currentX, int maxX)
    {
        int adjustedXSize = xSize * resolution;
        int adjustedZSize = zSize * resolution;
        float[,] heightMap = new float[adjustedXSize + 1, adjustedZSize + 1];
        uvs = new Vector2[(adjustedXSize + 1) * (adjustedZSize + 1)];
        Random rand = new Random(); 
        
        int vertexIndex = 0;
        for (int x = 0; x <= adjustedXSize; x++)
        {
            for (int z = 0; z <= adjustedZSize; z++)
            {
                uvs[vertexIndex] = new Vector2(x / (float)adjustedXSize, z / (float)adjustedZSize);
                heightMap[x, z] = Mathf.InverseLerp(0, 10, vertices[vertexIndex].y) + Random.Range(-1000, 1000) /(float)40000;
                vertexIndex++;
            }
        }

        mesh.uv = uvs;
        meshRenderer.sharedMaterial = new Material(meshRenderer.sharedMaterial);
        if (currentX < maxX/2) {
            meshRenderer.sharedMaterial.mainTexture = TextureGenerator.TextureFromHeightMap(heightMap, gradient);
        } else if (currentX == maxX/2) {
            meshRenderer.sharedMaterial.mainTexture = TextureGenerator.TextureFromHeightMap(heightMap, gradient, gradient2);
        } else {
            meshRenderer.sharedMaterial.mainTexture = TextureGenerator.TextureFromHeightMap(heightMap, gradient2);
        }
    }
}
