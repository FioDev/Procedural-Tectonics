using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Plate
{
    public float[,] heightMap;

    public int seed;
    public Vector2 speed;
    public Vector2 offset;
}
