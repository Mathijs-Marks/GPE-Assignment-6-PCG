using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    SerializedProperty tlTileProp;
    SerializedProperty tmTileProp;
    SerializedProperty trTileProp;
    SerializedProperty mlTileProp;
    SerializedProperty mmTileProp;
    SerializedProperty mrTileProp;
    SerializedProperty blTileProp;
    SerializedProperty bmTileProp;
    SerializedProperty brTileProp;

    [SerializeField] private bool showTiles = true;

    private void OnEnable()
    {
        tlTileProp = serializedObject.FindProperty("tlTile");
        tmTileProp = serializedObject.FindProperty("tmTile");
        trTileProp = serializedObject.FindProperty("trTile");
        mlTileProp = serializedObject.FindProperty("mlTile");
        mmTileProp = serializedObject.FindProperty("mmTile");
        mrTileProp = serializedObject.FindProperty("mrTile");
        blTileProp = serializedObject.FindProperty("blTile");
        bmTileProp = serializedObject.FindProperty("bmTile");
        brTileProp = serializedObject.FindProperty("brTile");
    }

    public override void OnInspectorGUI()
    {
        // Let's leverage the default implementation for later tile assignment.
        DrawDefaultInspector();

        showTiles = EditorGUILayout.Foldout(showTiles, "Tiles");

        if (showTiles)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("T", GUILayout.Width(20));
            EditorGUILayout.PropertyField(tlTileProp, new GUIContent(""));
            EditorGUILayout.PropertyField(tmTileProp, new GUIContent(""));
            EditorGUILayout.PropertyField(trTileProp, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("M", GUILayout.Width(20));
            EditorGUILayout.PropertyField(mlTileProp, new GUIContent(""));
            EditorGUILayout.PropertyField(mmTileProp, new GUIContent(""));
            EditorGUILayout.PropertyField(mrTileProp, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("B", GUILayout.Width(20));
            EditorGUILayout.PropertyField(blTileProp, new GUIContent(""));
            EditorGUILayout.PropertyField(bmTileProp, new GUIContent(""));
            EditorGUILayout.PropertyField(brTileProp, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        DungeonGenerator myScript = (DungeonGenerator)target;
        if (GUILayout.Button("Generate Dungeon"))
        {
            myScript.GenerateDungeon();
        }
    }
}
