using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colors, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colors);
        texture.Apply();
        //byte[] _bytes = texture.EncodeToPNG();
        //string _fullPath = "Assets/Images/pic.png";
        //System.IO.File.WriteAllBytes(_fullPath, _bytes);
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colors = new Color[width * height];
        for(int z = 0; z < height; z++)
        {
            for (int x = 0; x<width; x++)
            {
                colors[z * width + x] = Color.Lerp(Color.white, Color.black, heightMap[x, z]);
                //Debug.Log(heightMap[x, z].ToString());

            }
        }
        return TextureFromColorMap(colors, width, height);
    }
}
