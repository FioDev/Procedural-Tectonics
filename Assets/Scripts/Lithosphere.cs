using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Terrain types
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float heightVal;
    public Color colour;
}

public class Lithosphere : MonoBehaviour
{
    //Allows toggling between draw modes
    public enum DrawMode
    {
        NoiseMap, 
        ColourMap,
        Mesh
    }
    public DrawMode drawMode;

    //Set up map constants 
    [Range(1, 1000)] public int mapWidth; //Units are in cartesian coordinates
    [Range(1, 1000)] public int mapDepth;
    public float sampleScale;

    public int octaves;
    [Range(0, 1)] public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offsets;
    public float meshHeightMultiplier;

    public AnimationCurve meshHeightCurve;

    //Set up references
    public SurfaceDisplay display;

    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateSurface()
    {
        float[,] heightMap = Noise.GenerateNoiseMap(mapWidth, mapDepth, seed, sampleScale, octaves, persistance, lacunarity, offsets);

        //Colourmap implementation

        Color[] colourMap = new Color[mapWidth* mapDepth];

        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = heightMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].heightVal)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour; 
                        break;
                    }
                }
            }
        }
        //Guard clause against null mapdisplay
        if (display == null)
        {
            display = FindObjectOfType<SurfaceDisplay>();
        }

        //Set up toggling between draw modes
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        } else if  (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapDepth));
        } else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapDepth));
        }
    }

    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapDepth < 1)
        {
            mapDepth = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}
