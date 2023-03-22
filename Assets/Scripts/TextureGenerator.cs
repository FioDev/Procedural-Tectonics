using System.Collections;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        //Set filter modes so nothing is blurry
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp; //Also change this to clamp to avoid edges being blurred with other pixel data

        texture.SetPixels (colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        //Record the dimentions of the array
        int width = heightMap.GetLength(0);
        int depth = heightMap.GetLength(1);

        //Create texture to apply data to
        Texture2D texture = new Texture2D(width, depth);

        //Create an array of colours within the noisemap, and apply them to the texture at once
        //This is faster than itterating through all colours and all texture coordinates
        Color[] colors = new Color[width * depth];

        //Itterate colours
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                colors[j * width + i] = Color.Lerp(Color.black, Color.white, heightMap[i, j]);
            }
        }

        return TextureFromColourMap(colors, width, depth);
    }
}
