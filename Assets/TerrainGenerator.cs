using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] int Width = 50;
    [SerializeField] int Length = 50;
    [SerializeField] float perlinFrequencyX = 0.1f;
    [SerializeField] float perlinFrequencyZ = 0.1f;
    [SerializeField] float perlinNoiseStrength = 7f;

    enum TerrainStyle
    {
        TerrainColour,
        BlackToWhite,
        WhiteToBlack,
    }

    [SerializeField] TerrainStyle terrainStyle;

    Gradient TerrainGradient;
    Gradient BlackToWhiteGradient;
    Gradient WhiteToBlackGradient;

    Vector3[] vertices;
    int[] tris;
    Vector2[] uvs;
    Color[] colours;
    
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    NavMeshSurface navMeshSurface;
    MeshCollider meshCollider;
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
