using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        LevelManager levelManager = (LevelManager)target;
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Level"))
        {
            levelManager.ClearLevel();
            levelManager.GenerateLevel();
        }

        if (GUILayout.Button("Clear Level"))
        {
            levelManager.ClearLevel();
        }

        EditorUtility.SetDirty(levelManager);
    }
}
