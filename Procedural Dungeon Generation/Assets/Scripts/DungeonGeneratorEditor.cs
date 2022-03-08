using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Let's leverage the default implementation for later tile assignment.
        DrawDefaultInspector();

        DungeonGenerator myScript = (DungeonGenerator)target;
        if (GUILayout.Button("Generate Dungeon"))
        {
            myScript.GenerateDungeon();
        }
    }
}
