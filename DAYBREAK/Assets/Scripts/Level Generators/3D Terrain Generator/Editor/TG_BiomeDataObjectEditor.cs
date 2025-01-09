using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TG_BiomeDataObject))]
public class TG_BiomeDataObjectEditor : Editor
{
    SerializedProperty terrainLayer;
    SerializedProperty pathLayer;

    SerializedProperty terrainAF;
    SerializedProperty selectorAF;

    SerializedProperty structures;

    private void OnEnable()
    {
        terrainLayer = serializedObject.FindProperty("baseTerrainLayer");
        pathLayer = serializedObject.FindProperty("pathTerrainLayer");
        terrainAF = serializedObject.FindProperty("terrainAF");
        selectorAF = serializedObject.FindProperty("selectorAF");
        structures = serializedObject.FindProperty("structures");
    }

    public override void OnInspectorGUI()
    {
        TG_BiomeDataObject biomeData = (TG_BiomeDataObject)target;

        serializedObject.Update();

        terrainLayer.objectReferenceValue = EditorGUILayout.ObjectField("Base Terrain Layer", terrainLayer.objectReferenceValue, typeof(TerrainLayer), false);
        pathLayer.objectReferenceValue = EditorGUILayout.ObjectField("Path Terrain Layer", pathLayer.objectReferenceValue, typeof(TerrainLayer), false);

        EditorGUILayout.Space();

        // Show Amplitudes and Frequencies
        if (biomeData.showAFs = EditorGUILayout.Foldout(biomeData.showAFs, "Amplitudes and Frequencies"))
        {
            EditorGUILayout.Space(0.2f);
            EditorGUI.indentLevel++;

            // Terrain Amplitudes and Frequencies
            EditorGUILayout.BeginHorizontal();
            biomeData.showTerrainAF = EditorGUILayout.Foldout(biomeData.showTerrainAF, "Terrain Layers");
            terrainAF.arraySize = (int)Mathf.Clamp(EditorGUILayout.IntField(terrainAF.arraySize), 1, Mathf.Infinity);
            EditorGUILayout.EndHorizontal();

            if (biomeData.showTerrainAF)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < terrainAF.arraySize; i++) EditorGUILayout.PropertyField(terrainAF.GetArrayElementAtIndex(i), new GUIContent($"Layer {i + 1}"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space(2f);

            // Selector Amplitudes and Frequencies
            EditorGUILayout.BeginHorizontal();
            biomeData.showSelectorAF = EditorGUILayout.Foldout(biomeData.showSelectorAF, "Selector Layers");
            selectorAF.arraySize = (int)Mathf.Clamp(EditorGUILayout.IntField(selectorAF.arraySize), 1, Mathf.Infinity);
            EditorGUILayout.EndHorizontal();

            if (biomeData.showSelectorAF)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < selectorAF.arraySize; i++) EditorGUILayout.PropertyField(selectorAF.GetArrayElementAtIndex(i), new GUIContent($"Layer {i + 1}"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space(2f);
            EditorGUI.indentLevel--;
        }

        // Structures

        EditorGUILayout.BeginHorizontal();
        biomeData.showStructures = EditorGUILayout.Foldout(biomeData.showStructures, "Structures");
        structures.arraySize = (int)Mathf.Clamp(EditorGUILayout.IntField(structures.arraySize), 0, Mathf.Infinity);
        EditorGUILayout.EndHorizontal();

        if (biomeData.showStructure.Length != structures.arraySize)
        {
            bool[] newarray = new bool[structures.arraySize];
            for (int i = 0; i < structures.arraySize; i++) newarray[i] = (i < biomeData.showStructure.Length - 1) ? biomeData.showStructure[i] : false;

            biomeData.showStructure = new bool[structures.arraySize];
            for (int i = 0; i < biomeData.showStructure.Length; i++) biomeData.showStructure[i] = newarray[i];
        }

        if (biomeData.showStructures)
        {
            EditorGUI.indentLevel++;

            if (structures.arraySize > 0)
            {
                for (int i = 0; i < structures.arraySize; i++)
                {
                    EditorGUILayout.Space(2f);

                    TG_StructureDataObject structure = structures.GetArrayElementAtIndex(i).objectReferenceValue as TG_StructureDataObject;

                    EditorGUILayout.BeginHorizontal();
                    biomeData.showStructure[i] = EditorGUILayout.Foldout(biomeData.showStructure[i], structure == null ? "Structure " + (i + 1) : structure.name);
                    structures.GetArrayElementAtIndex(i).objectReferenceValue = EditorGUILayout.ObjectField(structures.GetArrayElementAtIndex(i).objectReferenceValue, typeof(TG_StructureDataObject), false);
                    EditorGUILayout.EndHorizontal();

                    if (biomeData.showStructure[i])
                    {
                        EditorGUI.indentLevel++;

                        if (structures.GetArrayElementAtIndex(i).objectReferenceValue)
                        {
                            EditorGUILayout.Space(2f);
                            Editor biomeEditor = CreateEditor(structures.GetArrayElementAtIndex(i).objectReferenceValue);
                            biomeEditor.OnInspectorGUI();
                        }
                        else
                        {
                            EditorGUILayout.LabelField("No Structure Data Added!");
                        }

                        EditorGUI.indentLevel--;
                    }
                }
            }
            else 
            { 
                EditorGUILayout.LabelField("No Structures Added!");
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
