using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TG_PathDataObject))]
public class TG_PathDataObjectEditior : Editor
{
    SerializedProperty pathTerrainLayer;
    SerializedProperty pathColors;
    SerializedProperty pathWidth;
    SerializedProperty pathFade;
    SerializedProperty controlPointPower;

    private void OnEnable()
    {
        pathTerrainLayer = serializedObject.FindProperty("layer");
        pathColors = serializedObject.FindProperty("pathColors");
        pathWidth = serializedObject.FindProperty("pathWidth");
        pathFade = serializedObject.FindProperty("pathFade");
        controlPointPower = serializedObject.FindProperty("controlPointPower");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        TG_PathDataObject tG_PathDataObject = (TG_PathDataObject)target;

        pathTerrainLayer.objectReferenceValue = EditorGUILayout.ObjectField("Terrain Layer", pathTerrainLayer.objectReferenceValue, typeof(TerrainLayer), false);

        EditorGUILayout.BeginHorizontal();
        tG_PathDataObject.showColors = EditorGUILayout.Foldout(tG_PathDataObject.showColors, pathColors.arraySize > 1 ? "Path Colors" : "Path Color");
        pathColors.arraySize = (int)Mathf.Clamp(EditorGUILayout.IntField(pathColors.arraySize), 1, Mathf.Infinity);
        EditorGUILayout.EndHorizontal();

        if (tG_PathDataObject.showColors)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Space(2f);

            for (int i = 0; i < pathColors.arraySize; i++)
            {
                pathColors.GetArrayElementAtIndex(i).colorValue = EditorGUILayout.ColorField($"Color {i + 1}", pathColors.GetArrayElementAtIndex(i).colorValue);
            }

            EditorGUI.indentLevel--;
        }
        else EditorGUILayout.Space(2f);

        EditorGUILayout.Space();

        pathWidth.floatValue = EditorGUILayout.FloatField("Path Width", pathWidth.floatValue);
        pathFade.floatValue = EditorGUILayout.FloatField("Path Fade", pathFade.floatValue);
        controlPointPower.floatValue = EditorGUILayout.FloatField("Control Power", controlPointPower.floatValue);

        serializedObject.ApplyModifiedProperties();
    }
}
