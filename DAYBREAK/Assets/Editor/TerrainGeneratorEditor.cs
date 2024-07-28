using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Terrain")) terrainGenerator.GenerateTerrain();
        if (GUILayout.Button("Clear Level")) terrainGenerator.ClearTerrain();

        GUILayout.Space(10);

        if (GUILayout.Button("Open Terrain Data Object") && terrainGenerator.terrainDataObject) AssetDatabase.OpenAsset(terrainGenerator.terrainDataObject);
    }
}
