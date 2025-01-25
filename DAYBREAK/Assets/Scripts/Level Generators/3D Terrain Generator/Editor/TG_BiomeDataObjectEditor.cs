using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TG_BiomeDataObject))]
public class TG_BiomeDataObjectEditor : Editor
{
    SerializedProperty terrainLayer;

    SerializedProperty biomeColors;

    SerializedProperty terrainAF;
    SerializedProperty selectorAF;

    SerializedProperty structures;
    SerializedProperty paths;

    private void OnEnable()
    {
        terrainLayer = serializedObject.FindProperty("baseTerrainLayer");
        biomeColors = serializedObject.FindProperty("biomeColors");
        terrainAF = serializedObject.FindProperty("terrainAF");
        selectorAF = serializedObject.FindProperty("selectorAF");
        structures = serializedObject.FindProperty("structures");
        paths = serializedObject.FindProperty("paths");
    }

    public override void OnInspectorGUI()
    {
        TG_BiomeDataObject biomeData = (TG_BiomeDataObject)target;

        serializedObject.Update();

        terrainLayer.objectReferenceValue = EditorGUILayout.ObjectField("Base Terrain Layer", terrainLayer.objectReferenceValue, typeof(TerrainLayer), false);

        EditorGUILayout.BeginHorizontal();
        biomeData.showColors = EditorGUILayout.Foldout(biomeData.showColors, biomeColors.arraySize > 1 ? "Biome Colors" : "Biome Color");
        biomeColors.arraySize = (int)Mathf.Clamp(EditorGUILayout.IntField(biomeColors.arraySize), 1, Mathf.Infinity);
        EditorGUILayout.EndHorizontal();

        if (biomeData.showColors)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Space(2f);

            for (int i = 0; i < biomeColors.arraySize; i++)
            {
                biomeColors.GetArrayElementAtIndex(i).colorValue = EditorGUILayout.ColorField($"Color {i + 1}", biomeColors.GetArrayElementAtIndex(i).colorValue);
            }

            EditorGUI.indentLevel--;
        }
        else EditorGUILayout.Space(2f);

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

        EditorGUILayout.Space(2f);

        EditorGUILayout.BeginHorizontal();
        biomeData.showPaths = EditorGUILayout.Foldout(biomeData.showPaths, "Paths");
        paths.arraySize = (int)Mathf.Clamp(EditorGUILayout.IntField(paths.arraySize), 0, Mathf.Infinity);
        EditorGUILayout.EndHorizontal();

        if (biomeData.showPath.Length != paths.arraySize)
        {
            bool[] newarray = new bool[paths.arraySize];
            for (int i = 0; i < paths.arraySize; i++) newarray[i] = (i < biomeData.showPath.Length - 1) ? biomeData.showPath[i] : false;

            biomeData.showPath = new bool[paths.arraySize];
            for (int i = 0; i < biomeData.showPath.Length; i++) biomeData.showPath[i] = newarray[i];
        }

        if (biomeData.showPaths)
        {
            EditorGUI.indentLevel++;

            if (paths.arraySize > 0)
            {
                for (int i = 0; i < paths.arraySize; i++)
                {
                    EditorGUILayout.Space(2f);

                    TG_PathDataObject path = paths.GetArrayElementAtIndex(i).objectReferenceValue as TG_PathDataObject;

                    EditorGUILayout.BeginHorizontal();
                    biomeData.showPath[i] = EditorGUILayout.Foldout(biomeData.showPath[i], path == null ? "Path " + (i + 1) : path.name);
                    paths.GetArrayElementAtIndex(i).objectReferenceValue = EditorGUILayout.ObjectField(paths.GetArrayElementAtIndex(i).objectReferenceValue, typeof(TG_PathDataObject), false);
                    EditorGUILayout.EndHorizontal();

                    if (biomeData.showPath[i])
                    {
                        EditorGUI.indentLevel++;

                        if (paths.GetArrayElementAtIndex(i).objectReferenceValue)
                        {
                            EditorGUILayout.Space(2f);
                            Editor pathEditor = CreateEditor(paths.GetArrayElementAtIndex(i).objectReferenceValue);
                            pathEditor.OnInspectorGUI();
                        }
                        else
                        {
                            EditorGUILayout.LabelField("No Path Data Added!");
                        }

                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Paths Added!");
            }

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
