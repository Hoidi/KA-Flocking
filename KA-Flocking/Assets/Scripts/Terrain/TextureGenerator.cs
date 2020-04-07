using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//inspired by this youtube vid: https://www.youtube.com/watch?v=RDQK1_SWFuc&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=4
public class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colors, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap, Gradient gradient)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colors = new Color[width * height];
        for(int z = 0; z < height; z++)
        {
            for (int x = 0; x<width; x++)
            {
                colors[z * width + x] = Color.Lerp(Color.white, Color.black, heightMap[x, z]); 
                colors[z * width + x] = gradient.Evaluate(heightMap[x, z]);

            }
        }
        return TextureFromColorMap(colors, width, height);
    }

    // Between two gradients
    public static Texture2D TextureFromHeightMap(float[,] heightMap, Gradient gradient1, Gradient gradient2)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colors = new Color[width * height];
        for(int z = 0; z < height; z++)
        {
            for (int x = 0; x<width; x++)
            {
                colors[z * width + x] = Color.Lerp(Color.white, Color.black, heightMap[x, z]); 
                colors[z * width + x] = Util.Gradient.Lerp(gradient1,gradient2,z/((float)height)).Evaluate(heightMap[x, z]);

            }
        }
        return TextureFromColorMap(colors, width, height);
    }
}
