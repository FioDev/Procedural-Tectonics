using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Rendering.Universal;
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
        Mesh,
        TectonicMesh
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

    //Heightmap to be rendered
    float[,] heightMap;

    //tectonic generation settings
    [Range(0, 1)] public float targetTectonicErosionHeight;
    [Range(0, 1)] public float erosionFactor;
    [Range(0, 1)] public float volcanismFactor;

    public List<Plate> plates;

    bool stepGeneration = false;
    bool smoothOnly = false;

    List<MagmaChamber> magmaChamberList = new List<MagmaChamber>();


    //Set up references to UI controlers
    public UIControlStatus statusUI;
    public UIControlSettings settingsUI;
    public CameraOrbit cam;


    private void Awake()
    {
        GenerateSurface();

        if (drawMode == DrawMode.TectonicMesh)
        {
            //Generate initial terrain data for plates
            
            for (int i = 0; i < plates.Count; i++)
            {
                plates[i] = UpdatePlateData(plates[i]);
            }
        }
    }

    private void Update()
    {
        //On space key down, step the generation processing.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stepGeneration = !stepGeneration;   
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            smoothOnly = !smoothOnly;
        }

        if (smoothOnly)
        {
            //make sure UI is broadcasting correct Status
            statusUI.UpdateStatus("smoothing");
            settingsUI.Hide();

            stepGeneration = false;
            heightMap = SmoothTerrain(heightMap);

            Color[] colourMap = new Color[mapWidth * mapDepth];

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

            //Display world data
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapDepth));
        }
        

        //All below this line is step generation process calling. 
        
        if (stepGeneration) 
        {
            //make sure UI is broadcasting correct Status
            statusUI.UpdateStatus("running");
            settingsUI.Hide();

            //Begin processing Tectonic Effects
            heightMap = ProcessTectonicEffects(heightMap);

            //Generate colours for world
            //Colourmap implementation

            Color[] colourMap = new Color[mapWidth * mapDepth];

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

            //Display world data
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapDepth));
        }

        if (!stepGeneration && !smoothOnly && !cam.hideGenUI)
        {
            //make sure UI is broadcasting correct Status
            statusUI.UpdateStatus("idle");
            settingsUI.Show();
        }

    }

    public void ApplySizeChange(int x, int y)
    {
        mapDepth = y;
        mapWidth = x;

        drawMode = DrawMode.Mesh;
        GenerateSurface();
        drawMode = DrawMode.TectonicMesh;
    }



    public Plate UpdatePlateData(Plate plate)
    {
        Plate processed = plate;
        processed.offset += (processed.speed * Time.deltaTime);
        processed.heightMap = Noise.GenerateNoiseMap(mapWidth, mapDepth, processed.seed, sampleScale, octaves, persistance, lacunarity, processed.offset);

        //check magma chambers for anything connected to current plate
        //null reference guard
        if (magmaChamberList!= null)
        {
            foreach (MagmaChamber chamber in magmaChamberList)
            {
                if (chamber.plateRelative != null)
                {
                    if (chamber.plateRelative == plate)
                    {
                        chamber.position += (processed.speed * Time.deltaTime);

                        //Catch "off the edge of the world" errors
                        if (chamber.position.x > mapWidth || chamber.position.x < -mapWidth || chamber.position.y > mapDepth || chamber.position.y < -mapDepth)
                        {
                            magmaChamberList.Remove(chamber); //remove chamber if out of bounds
                        }
                    }
                }
            }
        }
        

        return processed;
    }

    public float[,] ProcessTectonicEffects(float[,] old)
    {
        float[,] processed = old;

        //All tectonic logic goes here. 

        //Initially, erode all heightmap - pre output processing - to 0.25. This is "deep ocean". Deeper than this represents an oceanic rift.
        //This includes eroding upwards, so a lerp function can be used
        
        processed = PreTectonicErosion(processed);

        //Then, update plate movement data
        for (int i = 0; i < plates.Count; i++)
        {
            plates[i] = UpdatePlateData(plates[i]);
        }

        //Then, process positive height changes. 
        //Where plate 1 and plate 2 have height data of 0-0.2, and are oposite moving, this is pulling apart. Create a magma zone. 
        //Where plate 1 and plate 2 have height data of 0-0.5, and are towards moving, this is subduction. Create a magma zone.

        ProcessRifts(); //Add magma chamber data

        processed = ProcessMagmaChambers(processed); //Project magama chambers onto terrain

        processed = SmoothTerrain(processed);

        return processed;
    }

    private float[,] SmoothTerrain(float[,] old)
    {
        float[,] processed = old;

        //take note of values around, average them, and then lerp towards average by erosion factor.

        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //Calculate check range default: 1. 
                float average = 0;
                int count = 0;

                if (x-1 > 0)
                {
                    average += processed[x - 1, y];
                    count++;
                }
                if (x + 1 < mapWidth)
                {
                    average += processed[x + 1, y];
                    count++;
                }
                if (y - 1 > 0)
                {
                    average += processed[x, y - 1];
                    count++;
                }
                if (y + 1 < mapDepth)
                {
                    average += processed[x, y + 1];
                    count++;
                }

                average /= count;

                processed[x, y] = Mathf.Lerp(processed[x, y], average, erosionFactor);

                //If in the ocean, below a certain water altitude, further drop this. Forcefully, just annihilate the top off it by erosion factor
                if (processed[x, y] < 0.3)
                {
                    processed[x, y] = Mathf.Lerp(processed[x, y], targetTectonicErosionHeight, Time.deltaTime);
                    //since this can be really destructive, also force it above clamp height
                    processed[x, y] = Mathf.Clamp01(processed[x, y]);
                }
            }
        }

        return processed;
    }

    private float[,] ProcessMagmaChambers(float[,] old)
    {
        float[,] processed = old;

        foreach (MagmaChamber chamber in magmaChamberList)
        {
            float depositThisTick = (volcanismFactor / 100) * Time.deltaTime;
            int roundX = Mathf.RoundToInt(chamber.position.x);
            int roundY = Mathf.RoundToInt(chamber.position.y);


            chamber.size -= depositThisTick;

            //Boost size of magma if low level
            if (processed[roundX, roundY] < 0.3)
            {
                depositThisTick *= 2;
            }

            if (processed[roundX, roundY] < 0.8)
            {
                depositThisTick /= 2;
            }

            processed[roundX, roundY] += depositThisTick;

            //verify that processed terrain point is still under 1 tall, and over 0 short
            processed[roundX, roundY] = Mathf.Clamp01(processed[roundX, roundY]);


            //delete chamber if it is below 0 size
            if (chamber.size < 0)
            {
                magmaChamberList.Remove(chamber);
            }
        }

        return processed;
    }

    private void ProcessRifts()
    {
        

        //For each plate, look to see if two are going in even *glancingly* oposite directions.
        
        //plate origin
        for (int i = 0; i < plates.Count; i++)
        {
            //plate to compare
            for (int j = 0; j < plates.Count; j++)
            {
                if (i == j)
                {
                    continue; //Obviously, if comparing so self, rel. speed value will be zero. Ignore such values.
                }
                else
                {
                    //If plate is currently in negative speed, reverse comparison.
                    Vector2 temp;
                    Vector2 tempCompare = plates[j].speed;

                    temp.x = (plates[i].speed.x > 0) ? plates[i].speed.x : (plates[i].speed.x * -1f);
                    temp.y = (plates[i].speed.y > 0) ? plates[i].speed.y : (plates[i].speed.y * -1f);

                    if (temp.x != plates[i].speed.x)
                    {
                        tempCompare.x *= -1f;
                    }

                    if (temp.y != plates[i].speed.y)
                    {
                        tempCompare.y *= -1f;
                    }

                    //Sign flips complete, calculate mag

                    

                    //IF SPEEDS NEGATIVE, PLATES ARE CONVERGING. SUBDUCT RIFT
                    if (temp.x - tempCompare.x < 0 && temp.y - tempCompare.y < 0)
                    {
                        //for each point on each plate, compare sums. When both are oceanic, make magma chamber on "higher" height value, with size of difference.
                        for (int y = 0; y < mapDepth; y++)
                        {
                            for (int x = 0; x < mapWidth; x++)
                            {
                                if (plates[i].heightMap[x, y] < 0.5f && plates[j].heightMap[x, y] < 0.5f)
                                {
                                    //Congratulations, we are subducting. Lets make a new magma chamber

                                    MagmaChamber chamber = new MagmaChamber();

                                    chamber.position = new Vector2(x, y);
                                    chamber.plateRelative = (plates[i].heightMap[x, y] > plates[j].heightMap[x, y]) ? plates[i] : plates[j]; //relative position is highest terrain at point
                                    
                                    //Chamber size calculation
                                    if (plates[i].heightMap[x, y] > plates[j].heightMap[x, y])
                                    {
                                        chamber.size = plates[i].heightMap[x, y] - plates[j].heightMap[x, y];
                                    } else
                                    {
                                        chamber.size = plates[j].heightMap[x, y] - plates[i].heightMap[x, y];
                                    }

                                    magmaChamberList.Add(chamber);
                                }
                            }
                        }
                    } else if (temp.x - tempCompare.x > 0 && temp.y - tempCompare.y > 0) //If positive, causing oceanic rift. Do another rift. No reference plate. Size of speed pulling apart. 
                    {
                        for (int y = 0; y < mapDepth; y++)
                        {
                            for (int x = 0; x < mapWidth; x++)
                            {
                                if (plates[i].heightMap[x, y] < 0.2f && plates[j].heightMap[x, y] < 0.2f)
                                {
                                    //thats a rift, make the lava chamber
                                    MagmaChamber chamber = new MagmaChamber();
                                    chamber.position = new Vector2(x, y);
                                    chamber.plateRelative = null;

                                    chamber.size = Vector2.Distance(temp, tempCompare); //Size is how fast theyre pulling apart.

                                    magmaChamberList.Add(chamber);
                                }
                            }
                        }
                    }
              
                }
            }
        }
    }

    private float[,] PreTectonicErosion(float[,] old)
    {
        float[,] processed = old;

        //loop over each point and lerp towards 
        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                processed[x, y] = Mathf.Lerp(processed[x, y], targetTectonicErosionHeight, erosionFactor);
            }
        }

        return processed;
    }

    public void GenerateSurface()
    {
        

        if (drawMode != DrawMode.TectonicMesh)
        {
            heightMap = Noise.GenerateNoiseMap(mapWidth, mapDepth, seed, sampleScale, 0, persistance, lacunarity, offsets);
        } else
        {
            heightMap = Noise.GenerateNoiseMap(mapWidth, mapDepth, seed, sampleScale, 0, persistance, lacunarity, offsets);
            heightMap = ProcessTectonicEffects(heightMap);
        }

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
        } else if (drawMode == DrawMode.TectonicMesh)
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
