using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode { noiseMap, ColourMap, Mesh};
    public DrawMode drawMode;

    int mapWidth;
    int mapHeight;
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
    public AnimationCurve heightCurve;

    public NavMeshSurface navMeshSurface;
    public MeshCollider meshCollider;
    public MeshFilter meshFilter;

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
    private void TreesSpawn()
    {
        for (int i = 0; i < nOfTrees; i++)
        {
            bool itemTooClose;
            Vector3 treePosition;

            do
            {
                itemTooClose = false;

                // Generate random position within the specified boundaries
                float randomX = Random.Range(-26f, 26f); // X-axis: -26 to 26
                float randomY = Random.Range(-26f, 26f); // Z-axis: -26 to 26
                float height = SampleTerrain(randomX, randomY);

                treePosition = new Vector3(randomX, height, randomY);

                // Check for minimum distance from other trees
                foreach (GameObject tree in GameObject.FindGameObjectsWithTag("Tree"))
                {
                    if (Vector3.Distance(tree.transform.position, treePosition) < minDistance)
                    {
                        itemTooClose = true;
                        break;
                    }
                }
            } while (itemTooClose);

            Instantiate(treePrefab, treePosition, Quaternion.identity);
        }
    }
    private void HeartsSpawn()
    {
        for (int i = 0; i < nOfHearts; i++)
        {
            float randomX = Random.Range(0f, mapWidth);
            float randomY = Random.Range(0f, mapHeight);
            Vector3 randomPosition = new Vector3(randomX, 0f, randomY);

            bool ItemTooClose = false;
            foreach (GameObject heart in GameObject.FindGameObjectsWithTag("Heart"))
            {
                if (Vector3.Distance(heart.transform.position, randomPosition) < minDistance)
                {
                    ItemTooClose = true;
                    break;
                }
            }

            if (!ItemTooClose)
            {
                Vector3 heartPosition = new Vector3(randomX, SampleTerrain(randomX, randomY), randomY);
                Instantiate(heartPrefab, heartPosition, Quaternion.identity);
            }
        }
    }
    private void StarsSpawn()
    {
        for (int i = 0; i < nOfStars; i++)
        {
            float randomX = Random.Range(0f, mapWidth);
            float randomY = Random.Range(0f, mapHeight);
            Vector3 randomPosition = new Vector3(randomX, 0f, randomY);

            bool ItemTooClose = false;
            foreach (GameObject star in GameObject.FindGameObjectsWithTag("Star"))
            {
                if (Vector3.Distance(star.transform.position, randomPosition) < minDistance)
                {
                    ItemTooClose = true;
                    break;
                }
            }

            if (!ItemTooClose)
            {
                Vector3 starPosition = new Vector3(randomX, SampleTerrain(randomX, randomY), randomY);
                Instantiate(starPrefab, starPosition, Quaternion.identity);
            }
        }
    }
    private void GemsSpawn()
    {
        for (int i = 0; i < nOfGems; i++)
        {
            float randomX = Random.Range(0f, mapWidth);
            float randomY = Random.Range(0f, mapHeight);
            Vector3 randomPosition = new Vector3(randomX, 0f, randomY);

            bool ItemTooClose = false;
            foreach (GameObject gem in GameObject.FindGameObjectsWithTag("Gem"))
            {
                if (Vector3.Distance(gem.transform.position, randomPosition) < minDistance)
                {
                    ItemTooClose = true;
                    break;
                }
            }

            if (!ItemTooClose)
            {
                Vector3 gemPosition = new Vector3(randomX, SampleTerrain(randomX, randomY), randomY);
                Instantiate(gemPrefab, gemPosition, Quaternion.identity);
            }
        }
    }
    private void EnemyStarSpawn()
    {
        for (int i = 0; i < nOfEnemyStars; i++)
        {
            float randomX = Random.Range(0f, mapWidth);
            float randomY = Random.Range(0f, mapHeight);
            Vector3 randomPosition = new Vector3(randomX, 0f, randomY);

            bool ItemTooClose = false;
            foreach (GameObject enemyStar in GameObject.FindGameObjectsWithTag("EnemyStar"))
            {
                if (Vector3.Distance(enemyStar.transform.position, randomPosition) < minDistance)
                {
                    ItemTooClose = true;
                    break;
                }
            }

            if (!ItemTooClose)
            {
                Vector3 enemyStarPosition = new Vector3(randomX, SampleTerrain(randomX, randomY), randomY);
                Instantiate(enemyStarPrefab, enemyStarPosition, Quaternion.identity);
            }
        }
    }
    private void ChestSpawn()
    {
        for (int i = 0; i < nOfChests; i++)
        {
            float randomX = Random.Range(0f, mapWidth);
            float randomY = Random.Range(0f, mapHeight);
            Vector3 randomPosition = new Vector3(randomX, 0f, randomY);

            bool ItemTooClose = false;
            foreach (GameObject chest in GameObject.FindGameObjectsWithTag("Chest"))
            {
                if (Vector3.Distance(chest.transform.position, randomPosition) < minDistance)
                {
                    ItemTooClose = true;
                    break;
                }
            }

            if (!ItemTooClose)
            {
                Vector3 chestPosition = new Vector3(randomX, SampleTerrain(randomX, randomY), randomY);
                Instantiate(chestPrefab, chestPosition, Quaternion.identity);
            }
        }
    }
    private float SampleTerrain(float x, float y)
    {
        // Ensure x and y are within the bounds of the map
        int mapX = Mathf.Clamp((int)x, 0, mapWidth - 1);
        int mapY = Mathf.Clamp((int)y, 0, mapHeight - 1);

        // Generate the noise map if it doesn't exist
        if (noiseMap == null)
        {
            noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        }

        // Retrieve the height value from the noise map
        float heightValue = noiseMap[mapX, mapY];

        // Apply the height multiplier and the height curve
        return heightCurve.Evaluate(heightValue) * meshHeightMultiplier;
    }

    // Add a private field to store the noise map
    private float[,] noiseMap;
}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
