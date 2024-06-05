using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        LevelManager levelManager = (LevelManager)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Clear Level")) levelManager.ClearLevel();
    }
}
