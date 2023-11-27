using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode { noiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    int mapWidth;
    int mapHeight;
    public float noiseScale;

    public bool autoUpdate;

    public TerrainType[] regions;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;


    public NavMeshSurface navMeshSurface;
    public MeshCollider meshCollider;
    public MeshFilter meshFilter;

    public GameObject playerPrefab;
    public GameObject heartPrefab;
    public GameObject starPrefab;
    public GameObject treePrefab;
    public GameObject gemPrefab;
    public GameObject enemyStarPrefab;
    public GameObject chestPrefab;

    public int nOfHearts = 5;
    public int nOfStars = 5;
    public int nOfTrees = 10;
    public int nOfGems = 5;
    public int nOfEnemyStars = 5;
    public int nOfChests = 5;

    public float minDistance = 5f;
    public float maxDistance = 15f;


    private void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        meshCollider = GetComponent<MeshCollider>();

        meshFilter = GetComponent<MeshFilter>();
        meshCollider.sharedMesh = meshFilter.sharedMesh;

        HeartsSpawn();
        StarsSpawn();
        TreesSpawn();
        GemsSpawn();
        EnemyStarSpawn();
        ChestSpawn();
    }


    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[y, x];
                {
                    for (int i = 0; i < regions.Length; i++)
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
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, meshCollider), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
    }
    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
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
    private void HeartsSpawn()
    {
        SpawnItems(nOfHearts, heartPrefab, "Heart");
    }
    private void StarsSpawn()
    {
        SpawnItems(nOfStars, starPrefab, "Star");
    }

    private void TreesSpawn()
    {
        SpawnItems(nOfTrees, treePrefab, "Tree");
    }

    private void GemsSpawn()
    {
        SpawnItems(nOfGems, gemPrefab, "Gem");
    }

    private void EnemyStarSpawn()
    {
        SpawnItems(nOfEnemyStars, enemyStarPrefab, "EnemyStar");
    }

    private void ChestSpawn()
    {
        SpawnItems(nOfChests, chestPrefab, "Chest");
    }

    private void SpawnItems(int numberOfItems, GameObject prefab, string tag)
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            bool itemTooClose;
            Vector3 itemPosition;

            do
            {
                itemTooClose = false;
                float randomX = Random.Range(-26f, 26f);
                float randomZ = Random.Range(-26f, 26f);

                // Calculate the correct height at the position
                float height = TerrainHeightAtPoint(randomX, randomZ);

                // Use this height for the y-coordinate
                itemPosition = new Vector3(randomX, height, randomZ);

                foreach (GameObject item in GameObject.FindGameObjectsWithTag(tag))
                {
                    if (Vector3.Distance(item.transform.position, itemPosition) < minDistance)
                    {
                        itemTooClose = true;
                        break;
                    }
                }
            } while (itemTooClose);

            Instantiate(prefab, itemPosition, Quaternion.identity);
        }
    }

   
    private float TerrainHeightAtPoint(float x, float z)
    {
        RaycastHit hit;
        float y = 0;
        int terrainLayer = 1 << LayerMask.NameToLayer("TerrainLayer"); // Replace with your terrain's layer
        Vector3 rayStart = new Vector3(x, 1000, z);

        // Debug line for visualization (visible in the Scene view)
        Debug.DrawLine(rayStart, rayStart + Vector3.down * 1500, Color.red, 5f);

        // Cast a ray straight down from the start point with layer mask
        if (Physics.Raycast(rayStart, Vector3.down, out hit, 1500, terrainLayer))
        {
            y = hit.point.y;
        }
        else
        {
            // Handle the case where the ray doesn't hit the terrain
        }

        return y;
    }
}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}