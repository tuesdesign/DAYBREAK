using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.Tilemaps;

public class LevelStructureCreatorTool : EditorWindow
{
    Tilemap tilemap;
    string structureName;
    Vector3Int structureOffset;
    LevelStructureObject levelStructure;

    [MenuItem("Tools/Level Structure Creator")]
    public static void ShowWindow()
    {
        GetWindow<LevelStructureCreatorTool>("Level Structure Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a new level structure", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        tilemap = EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true) as Tilemap;
        structureName = EditorGUILayout.TextField("Structure Name", structureName);

        EditorGUILayout.Space();

        if (GUILayout.Button("Center View"))
        {
            SceneView.lastActiveSceneView.AlignViewToObject(tilemap.transform);
        }

        EditorGUILayout.Space();

        structureOffset = EditorGUILayout.Vector3IntField("Structure Offset", structureOffset);

        if (GUILayout.Button("Offset Structure"))
        {
            OffsetStructure(structureOffset);
        }

        if (GUILayout.Button("Center Structure"))
        {
            Vector2Int tempPositivePos = new Vector2Int();
            Vector2Int tempNegativePos = new Vector2Int();
            bool first = true;

            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos))
                {
                    if (first)
                    {
                        tempPositivePos = new Vector2Int(pos.x, pos.y);
                        tempNegativePos = new Vector2Int(pos.x, pos.y);
                        first = false;
                    }

                    if (pos.x > tempPositivePos.x) tempPositivePos.x = pos.x;
                    if (pos.y > tempPositivePos.y) tempPositivePos.y = pos.y;

                    if (pos.x < tempNegativePos.x) tempNegativePos.x = pos.x;
                    if (pos.y < tempNegativePos.y) tempNegativePos.y = pos.y;
                }
            }

            OffsetStructure(-(Vector3Int)(tempPositivePos + tempNegativePos) / 2);
        }

        void OffsetStructure(Vector3Int newTilemapOffset)
        {
            if (newTilemapOffset == Vector3Int.zero) return;

            Tilemap tempTilemap = Instantiate(tilemap);
            tilemap.ClearAllTiles();
            
            foreach (Vector3Int position in tempTilemap.cellBounds.allPositionsWithin)
            {
                if (tempTilemap.HasTile(position))
                {
                    tilemap.SetTile(position + newTilemapOffset, tempTilemap.GetTile(position));
                }
            }

            DestroyImmediate(tempTilemap.gameObject);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Level Structure Object"))
        {
            bool structureIsCurrentlySelected = this.levelStructure.name == structureName;

            LevelStructureObject levelStructure = ScriptableObject.CreateInstance<LevelStructureObject>();
            List<LevelStructureObject.StructureTile> structureTiles = new List<LevelStructureObject.StructureTile>();

            levelStructure.gridZCellSize = tilemap.GetComponentInParent<Grid>().cellSize.z;

            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(position))
                {
                    structureTiles.Add(new LevelStructureObject.StructureTile(position, (Tile)tilemap.GetTile(position)));
                    levelStructure.structureTiles = structureTiles.ToArray();
                }
            }

            AssetDatabase.CreateAsset(levelStructure, $"Assets/Scripts/Level Generator/Level Structures/Structures/{structureName}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (structureIsCurrentlySelected) this.levelStructure = levelStructure;
        }

        if (GUILayout.Button("Clear Tilemap"))
        {
            tilemap.ClearAllTiles();
        }

        EditorGUILayout.Space();

        levelStructure = EditorGUILayout.ObjectField(structureName, levelStructure, typeof(LevelStructureObject), true) as LevelStructureObject;

        if (GUILayout.Button("Place Structure"))
        {
            if (levelStructure != null)
            {
                structureName = levelStructure.name;
                levelStructure.PlaceStructure(Vector3Int.zero, tilemap);
            }
        }
    }
}
