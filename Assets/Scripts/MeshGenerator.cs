using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
<<<<<<< HEAD
    public static MeshData GenerateTerrainMesh(float[,] heightMap,float heightMultiplier, AnimationCurve heightCurve )
=======
    public static MeshData GenerateTerrainMesh(float[,] heightMap)
>>>>>>> 63a87faf0cae0cb0b0e69bdadb109c841a4b6f73
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData (width, height);
        int vertexIndex = 0;

        for (int y=0; y <height; y++)
        {
            for (int x = 0; x < width; x++)
            {
<<<<<<< HEAD
                meshData.vertices[vertexIndex] = new Vector3(topLeftX+ x, heightCurve.Evaluate(heightMap[y, x])*heightMultiplier,topLeftZ- y);
=======
                meshData.vertices[vertexIndex] = new Vector3(topLeftX+ x, heightMap[y, x],topLeftZ- y);
>>>>>>> 63a87faf0cae0cb0b0e69bdadb109c841a4b6f73
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);

                if (x< width-1 && y < height -1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width +1, vertexIndex, vertexIndex +1);
                }
                vertexIndex++;
            }
        }
        return meshData;
    }

}
public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth-1) * (meshHeight-1)*6];
    }
    public void AddTriangle(int a, in int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;  
        mesh.RecalculateNormals();
        return mesh;
    }


}