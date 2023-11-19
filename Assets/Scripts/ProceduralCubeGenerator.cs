using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCubeGenerator : MonoBehaviour
{

    public Vector3[] vertices = {
        new Vector3(0,0,0),//front face vertices
        new Vector3(1,0,0),
        new Vector3(1,1,0),
        new Vector3(0,1,0),


        new Vector3(0,0,1),//rear face vertices
        new Vector3(1,0,1),
        new Vector3(1,1,1),
        new Vector3(0,1,1),

        new Vector3(0,1,0),//left face extra vertices
        new Vector3(0,1,1),

        new Vector3(1,1,0),//right face extra vertices
        new Vector3(1,1,1),

        new Vector3(0,1,0),//far end face extra vertices
        new Vector3(1,1,0)
    };

    public int[] tris = {
        0, 2, 1, //front face
	    0, 3, 2,

        5, 7, 4, //rear face
	    5, 6, 7,

        3, 6, 2, //top face
	    3, 7, 6,

        0, 5, 4, //bottom face
	    0, 1, 5,

        4, 3, 0, //left face
	    4, 7, 3,

        1, 6, 5, //right face
	    1, 2, 6

    };

    // Start is called before the first frame update
    void Start()
    {

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.Optimize();
        mesh.RecalculateNormals();
    }

}
