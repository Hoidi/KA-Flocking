using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A chunk with vertice colors which makes it look smooth. 
public class SmoothChunk : Chunk
{
    protected Color[] colors;
    protected override void updateColors(int currentX, int maxX)
    {
        //sets the colors of the mesh from a gradiant depending on the height.
        colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            //float adjustedHeight = Mathf.InverseLerp(minHeight * height, maxHeight * height, vertices[i].y);
            float adjustedHeight = Mathf.InverseLerp(colorMin, colorMax, vertices[i].y);
            colors[i] = gradient.Evaluate(adjustedHeight);
        }
        mesh.colors = colors;
    }
}
