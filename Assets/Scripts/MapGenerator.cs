using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public bool autoUpdate;
    public int octaves;
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight,seed, noiseScale,octaves,persistance,lacunarity,offset);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }
    private void OnValidate()
    {
        if (mapWidth <1)
        {
            mapWidth = 1;
        }
        if (mapHeight <1)
        {
            mapHeight = 1;
        }
    }
}
