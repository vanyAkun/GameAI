using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor :   Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        // Draw the default inspector. 
        // Remove the redundant call to DrawDefaultInspector.
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        // Add a button to manually trigger map generation.
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}


