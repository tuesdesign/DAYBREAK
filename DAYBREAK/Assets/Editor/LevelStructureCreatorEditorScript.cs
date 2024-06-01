using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(LevelStructureCreator))]
public class LevelStructureCreatorEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        LevelStructureCreator target = (LevelStructureCreator)this.target;

        DrawDefaultInspector();

        if (GUILayout.Button("Place Tile at (0, 0, 0)"))
        {
            target.PlaceTileAtPosition(target.tilemap, target.debugTile, new Vector3Int(0, 0, 0));
        }
    }
}
