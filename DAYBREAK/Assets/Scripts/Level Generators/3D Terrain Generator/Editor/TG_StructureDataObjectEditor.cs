using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TG_StructureDataObject))]
public class TG_StructureDataObjectEditor : Editor
{
    SerializedProperty obj;
    SerializedProperty offestY;
    SerializedProperty radius;

    private void OnEnable()
    {
        obj = serializedObject.FindProperty("obj");
        offestY = serializedObject.FindProperty("offestY");
        radius = serializedObject.FindProperty("radius");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        obj.objectReferenceValue = EditorGUILayout.ObjectField("Structure Prefab", obj.objectReferenceValue, typeof(GameObject), false);
        offestY.floatValue = EditorGUILayout.FloatField("Transform Y Offset", offestY.floatValue);
        radius.intValue = EditorGUILayout.IntField("Border Radius", radius.intValue);

        serializedObject.ApplyModifiedProperties();
    }
}
