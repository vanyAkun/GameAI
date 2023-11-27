using System.Collections;
using System.Collections.Generic;

using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;

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

    public GameObject player;
    public GameObject NPC;

    public GameObject heartPrefab;
    public GameObject starPrefab;
    public GameObject treePrefab;
    public GameObject gemPrefab;
    public int HeartAmount = 5;
    public int TreeAmount = 10;
    public int GemAmount = 3;
    public int StarAmount = 7;



    private void Awake()
    {
       navMeshSurface = GetComponent<NavMeshSurface>();
        meshCollider = GetComponent<MeshCollider>();

        meshFilter = GetComponent<MeshFilter>();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
        SpawnPlayerOnTerrain();
        float minDistance = 5f;
        float maxDistance = 15f;// Adjust this value as needed
        SpawnItemOnTerrain(heartPrefab, HeartAmount, "Heart", minDistance   , maxDistance);
        SpawnItemOnTerrain(treePrefab, TreeAmount, "Tree", minDistance, maxDistance);
        SpawnItemOnTerrain(gemPrefab, GemAmount, "Gem", minDistance, maxDistance);
        SpawnItemOnTerrain(starPrefab, StarAmount, "Star", minDistance, maxDistance);
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
    private void SpawnPlayerOnTerrain()
    {
        // Randomly choose a position within the specified range
        float x = Random.Range(-26f, 26f); // Range for x coordinate
        float z = Random.Range(-26f, 26f); // Range for z coordinate

        // Get the terrain height at this point
        float y = GetTerrainHeightAtPoint(x, z);

        // Spawn the player at the calculated position
        Vector3 spawnPosition = new Vector3(x, y, z);
        Instantiate(player, spawnPosition, Quaternion.identity);
    }
    private void SpawnItemOnTerrain(GameObject itemPrefab, int amount, string tag, float minDistance, float maxDistance)
    {
        for (int i = 0; i < amount; i++)
        {
            bool tooClose;
            Vector3 spawnPosition;
            do
            {
                tooClose = false;
                float x = Random.Range(-26f, 26f);
                float z = Random.Range(-26f, 26f);
                float y = GetTerrainHeightAtPoint(x, z);
                spawnPosition = new Vector3(x, y, z);

                foreach (GameObject item in GameObject.FindGameObjectsWithTag(tag))
                {
                    if (Vector3.Distance(item.transform.position, spawnPosition) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }
            }
            while (tooClose); // Repeat the loop until a suitable position is found

            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }
    private float GetTerrainHeightAtPoint(float x, float z)
{
    Vector3 rayStart = new Vector3(x, 1000f, z);
    RaycastHit hit;

    // Debugging line: Red if the ray doesn't hit the terrain, green if it does
    Color debugLineColor = Color.red;

    if (Physics.Raycast(rayStart, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
    {
        debugLineColor = Color.green;
        // Draw a line to show where the raycast hits the terrain
        Debug.DrawLine(rayStart, hit.point, debugLineColor, 2f);
        return hit.point.y;
    }
    else
    {
        // Draw a ray to show the raycast direction
        Debug.DrawRay(rayStart, Vector3.down * 1000f, debugLineColor, 2f);
        return 0f;
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
