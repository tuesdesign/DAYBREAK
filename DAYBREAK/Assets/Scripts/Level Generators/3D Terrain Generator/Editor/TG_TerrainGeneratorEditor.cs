using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator)), CanEditMultipleObjects]
public class TerrainGeneratorEditor : Editor
{
    SerializedProperty terrainDataObject;
    SerializedProperty terrainSeed;
    SerializedProperty terrainMaterial;

    Editor terrainDataObjectEditor;

    bool showTerrainDataObject = false;

    private void OnEnable()
    {
        terrainDataObject = serializedObject.FindProperty("terrainDataObject");
        terrainDataObjectEditor = CreateEditor(terrainDataObject.objectReferenceValue);

        terrainSeed = serializedObject.FindProperty("_seed");
        terrainMaterial = serializedObject.FindProperty("terrainMaterial");
    }

    public override void OnInspectorGUI()
    {
        // level data
        // seed
        // size

        // biomes
        // painting

        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        // Generate terrain buttons
        if (GUILayout.Button("Generate Terrain")) terrainGenerator.GenerateTerrain();
        if (GUILayout.Button("Clear Level")) terrainGenerator.ClearTerrain();
        EditorGUILayout.Space();

        // Seed field
        string seed = terrainSeed.stringValue = EditorGUILayout.TextField("Seed", terrainSeed.stringValue);
        if (!int.TryParse(seed, out int intSeed))
            if (GUILayout.Button($"Copy Seed After Conversion: {seed.GetHashCode()}"))
            {
                TextEditor te = new TextEditor();
                te.text = seed.GetHashCode().ToString();
                te.SelectAll();
                te.Copy();
            }

        EditorGUILayout.BeginHorizontal();

        showTerrainDataObject = EditorGUILayout.Foldout(showTerrainDataObject, "Terrain Data Object");
        terrainDataObject.objectReferenceValue = EditorGUILayout.ObjectField(terrainDataObject.objectReferenceValue, typeof(TG_TerrainDataObject), false);

        EditorGUILayout.EndHorizontal();

        // if terrain data object updated in inspector, update the editor
        if (terrainDataObject.objectReferenceValue != null)
            if (terrainDataObjectEditor == null || terrainDataObjectEditor.target != terrainDataObject.objectReferenceValue)
                terrainDataObjectEditor = CreateEditor(terrainDataObject.objectReferenceValue);

        // Show the terrain data object editor
        if (showTerrainDataObject)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Space(2f);

            terrainDataObjectEditor.OnInspectorGUI();
            terrainMaterial.objectReferenceValue = EditorGUILayout.ObjectField("Terrain Material", terrainMaterial.objectReferenceValue, typeof(Material), false);
            EditorGUILayout.Space();

            EditorGUI.indentLevel--;
        }

        // Show the debug options
        EditorGUILayout.PropertyField(serializedObject.FindProperty("debugOptions"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
