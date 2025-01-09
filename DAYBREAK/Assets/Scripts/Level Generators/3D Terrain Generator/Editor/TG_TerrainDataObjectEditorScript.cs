using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TG_TerrainDataObject))]
public class TerrainDataObjectEditorScript : Editor
{
    SerializedProperty seed;
    SerializedProperty mapSize;

    static bool showBiomes = false;
    static bool[] showBiome = new bool[0];
    SerializedProperty biomes;

    // review this later ------------------------------------
    static bool showPaintingSettings = false;
    SerializedProperty biomeSeperation;
    SerializedProperty biomePaintSeperation;

    static bool showStructureSettings = false;
    SerializedProperty structureDensity;
    SerializedProperty structureEdgeBuffer;
    SerializedProperty structureEdgeCurve;
    SerializedProperty structureSeperationBuffer;
    SerializedProperty structureElevationAboveWater;

    static bool showIslandSettings = false;
    SerializedProperty waterLevel;
    SerializedProperty islandSize;

    SerializedProperty islandSlope;
    SerializedProperty islandSlopeStrength;

    SerializedProperty islandBorderRoughness;
    SerializedProperty islandBorderRoughnessStrength;

    private void OnEnable()
    {
        seed = serializedObject.FindProperty("seed");
        mapSize = serializedObject.FindProperty("mapSize");

        biomes = serializedObject.FindProperty("biomes");

        biomeSeperation = serializedObject.FindProperty("biomeSeperation");
        biomePaintSeperation = serializedObject.FindProperty("biomePaintSeperation");

        structureDensity = serializedObject.FindProperty("structureDensity");
        structureEdgeBuffer = serializedObject.FindProperty("structureEdgeBuffer");
        structureEdgeCurve = serializedObject.FindProperty("structureEdgeCurve");
        structureSeperationBuffer = serializedObject.FindProperty("structureSeperationBuffer");
        structureElevationAboveWater = serializedObject.FindProperty("structureElevationAboveWater");

        waterLevel = serializedObject.FindProperty("waterLevel");
        islandSize = serializedObject.FindProperty("islandSize");

        islandSlope = serializedObject.FindProperty("islandSlope");
        islandSlopeStrength = serializedObject.FindProperty("islandSlopeStrength");

        islandBorderRoughness = serializedObject.FindProperty("islandBorderRoughness");
        islandBorderRoughnessStrength = serializedObject.FindProperty("islandBorderRoughnessStrength");
    }

    public override void OnInspectorGUI()
    {
        seed.stringValue = EditorGUILayout.TextField("Data Object Seed", seed.stringValue);
        mapSize.vector2IntValue = EditorGUILayout.Vector2IntField("Map Size", mapSize.vector2IntValue);

        EditorGUILayout.BeginHorizontal();

        showBiomes = EditorGUILayout.Foldout(showBiomes, "Biomes");
        biomes.arraySize = (int)Mathf.Clamp(EditorGUILayout.IntField(biomes.arraySize), 1, Mathf.Infinity);
        
        if (showBiome.Length != biomes.arraySize)
        {
            bool[] newarray = new bool[biomes.arraySize];
            showBiome.CopyTo(newarray, 0);
            showBiome = new bool[biomes.arraySize];
            newarray.CopyTo(showBiome, 0);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(2f);

        if (showBiomes)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < biomes.arraySize; i++)
            {
                TG_BiomeDataObject biome = biomes.GetArrayElementAtIndex(i).objectReferenceValue as TG_BiomeDataObject;
                string biomeName = biome == null ? "Biome " + (i + 1) : biome.name;

                EditorGUILayout.BeginHorizontal();

                showBiome[i] = EditorGUILayout.Foldout(showBiome[i], biomeName);
                biomes.GetArrayElementAtIndex(i).objectReferenceValue = EditorGUILayout.ObjectField(biomes.GetArrayElementAtIndex(i).objectReferenceValue, typeof(TG_BiomeDataObject), false);

                EditorGUILayout.EndHorizontal();

                if (showBiome[i])
                {
                    EditorGUI.indentLevel++;

                    if (biomes.GetArrayElementAtIndex(i).objectReferenceValue)
                    {
                        EditorGUILayout.Space(2f);
                        Editor biomeEditor = CreateEditor(biomes.GetArrayElementAtIndex(i).objectReferenceValue);
                        biomeEditor.OnInspectorGUI();
                    }
                    else
                    {
                        EditorGUILayout.LabelField("No Biome Data Added!");
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(2f);
            }

            EditorGUI.indentLevel--;
        }

        if (showPaintingSettings = EditorGUILayout.Foldout(showPaintingSettings, "Biome Painting Settings"))
        {
            EditorGUI.indentLevel++;

            biomeSeperation.floatValue = EditorGUILayout.FloatField("Biome Seperation", biomeSeperation.floatValue);
            biomePaintSeperation.floatValue = EditorGUILayout.FloatField("Biome Paint Seperation", biomePaintSeperation.floatValue);

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }

        if (showStructureSettings = EditorGUILayout.Foldout(showStructureSettings, "Structure Generation Settings"))
        {
            EditorGUI.indentLevel++;

            structureDensity.floatValue = EditorGUILayout.FloatField("Structure Density", structureDensity.floatValue);
            structureEdgeBuffer.intValue = EditorGUILayout.IntField("Structure Edge Buffer", structureEdgeBuffer.intValue);
            structureEdgeCurve.animationCurveValue = EditorGUILayout.CurveField("Structure Edge Curve", structureEdgeCurve.animationCurveValue);
            structureSeperationBuffer.floatValue = EditorGUILayout.FloatField("Structure Seperation Buffer", structureSeperationBuffer.floatValue);
            structureElevationAboveWater.floatValue = EditorGUILayout.FloatField("Structure Elevation Above Water", structureElevationAboveWater.floatValue);

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }

        if (showIslandSettings = EditorGUILayout.Foldout(showIslandSettings, "Island Generation Settings"))
        {
            EditorGUI.indentLevel++;

            waterLevel.floatValue = EditorGUILayout.FloatField("Water Level", waterLevel.floatValue);
            islandSize.floatValue = EditorGUILayout.FloatField("Island Size", islandSize.floatValue);

            islandSlope.animationCurveValue = EditorGUILayout.CurveField("Island Slope", islandSlope.animationCurveValue);
            islandSlopeStrength.floatValue = EditorGUILayout.FloatField("Island Slope Strength", islandSlopeStrength.floatValue);

            islandBorderRoughness.floatValue = EditorGUILayout.FloatField("Island Border Roughness", islandBorderRoughness.floatValue);
            islandBorderRoughnessStrength.floatValue = EditorGUILayout.FloatField("Island Border Roughness Strength", islandBorderRoughnessStrength.floatValue);

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
