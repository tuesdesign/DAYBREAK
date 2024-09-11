using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(TerrainGenerator)), CanEditMultipleObjects]
public class TerrainGeneratorEditor : Editor
{
    SerializedProperty terrainDataObject;

    Editor terrainDataObjectEditor;

    bool showTerrainDataObject = false;

    private void OnEnable()
    {
        terrainDataObject = serializedObject.FindProperty("terrainDataObject");
        terrainDataObjectEditor = CreateEditor(terrainDataObject.objectReferenceValue);
    }

    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        // Terrain data object field
        terrainDataObject.objectReferenceValue = EditorGUILayout.ObjectField("Terrain Data Object", terrainDataObject.objectReferenceValue, typeof(TerrainDataObject), false);
        EditorGUILayout.Space();

        // Generate terrain buttons
        if (GUILayout.Button("Generate Terrain")) terrainGenerator.GenerateTerrain();
        if (GUILayout.Button("Clear Level")) terrainGenerator.ClearTerrain();
        EditorGUILayout.Space();

        // if terrain data object updated in inspector, update the editor
        if (terrainDataObject.objectReferenceValue != null)
            if (terrainDataObjectEditor == null || terrainDataObjectEditor.target != terrainDataObject.objectReferenceValue)
                terrainDataObjectEditor = CreateEditor(terrainDataObject.objectReferenceValue);

        // Show the terrain data object editor
        if (showTerrainDataObject = EditorGUILayout.Foldout(showTerrainDataObject, "Terrain Data Values")) terrainDataObjectEditor.OnInspectorGUI();
        EditorGUILayout.Space();

        // Show the debug options
        EditorGUILayout.PropertyField(serializedObject.FindProperty("debugOptions"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
