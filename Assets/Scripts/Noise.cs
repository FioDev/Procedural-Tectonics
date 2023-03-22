using System.Collections;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapDepth, int seed, float noiseScale, int octaves, float persistance, float lacunarity, Vector2 manualOffset)
    {
        float[,] noiseMap = new float[mapWidth, mapDepth];

        //Set up seed to generate different data from a chosen seed for each octave
        //Vector2 , save seeds and XY coordinate offsets to array locations coresponding to octave.
        System.Random rngsus = new System.Random(seed);
        Vector2[] offsets = new Vector2[octaves];
        
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rngsus.Next(-100000, 100000) + manualOffset.x;
            float offsetY = rngsus.Next(-100000, 100000) + manualOffset.y;

            offsets[i] = new Vector2(offsetX, offsetY);
        }

        //Guard against division by 0 in the scale later
        if (noiseScale <= 0) { noiseScale = 0.0001f; }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //For scale zooming to centre, calculate half and zoom to there
        float halfWidth = mapWidth / 2f;
        float halfDepth = mapDepth / 2f;

        //itterates over noisemap for its dimensions, and samples perlin.
        //Also divides ints by the scale, to make sure that the outputted perlin is not universally flat
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapDepth; j++)
            {
                float amp = 1;
                float freq = 1;
                float noiseHeight = 0;

                for (int k = 0; k < octaves; k++)
                {
                    float sampleX = (j - halfWidth ) / noiseScale * freq + offsets[k].x;
                    float sampleY = (i - halfDepth ) / noiseScale * freq + offsets[k].y;

                    //Gets perlin noise in the range of -1 to 1 - allowing for negative height on terrain generatioon.
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amp; //Sets noise height to be sample, plus the amplitude, which decreases for every loop over an octave.

                    //Cycle noise values down (or up) depending on noise amplitude persistance
                    //And also frequency lacunarity values.
                    amp *= persistance;
                    freq *= lacunarity;
                }

                //Update max and min noiseheight statistics
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                } else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                //Apply noise height to noise map
                noiseMap[i, j] = noiseHeight;
            }
        }

        // Inverse lerp between min and max noise value
        //This is a normalisation
        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
