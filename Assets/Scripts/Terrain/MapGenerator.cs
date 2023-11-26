using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode { noiseMap, ColourMap, Mesh};
    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public bool autoUpdate;

    public TerrainType[] regions;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public NavMeshSurface navMeshSurface;
    public MeshCollider meshCollider;
    public MeshFilter meshFilter;

    public GameObject heartPrefab;
    public GameObject starPrefab;
    public GameObject treePrefab;
    public GameObject gemPrefab;
    public GameObject enemyGemPrefab;
    public GameObject chestPrefab;


    private void Awake()
    {
       navMeshSurface = GetComponent<NavMeshSurface>();
        meshCollider = GetComponent<MeshCollider>();

        meshFilter = GetComponent<MeshFilter>();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight,seed, noiseScale,octaves,persistance,lacunarity,offset);

        Color[]  colourMap = new Color[ mapWidth * mapHeight];
        for (int y=0; y <mapHeight; y++)
        {
            for (int x=0; x <mapWidth; x++)
            {
                float currentHeight = noiseMap[y,x];
                {
                    for (int i= 0; i <regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colourMap[y * mapWidth + x] = regions[i].colour;
                            break;
                        }
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
       
        if (drawMode == DrawMode.noiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
          
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap,mapWidth,mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap,meshHeightMultiplier,meshHeightCurve,meshCollider), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
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
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves<0)
        {
            octaves = 0;
        }
    }
}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
