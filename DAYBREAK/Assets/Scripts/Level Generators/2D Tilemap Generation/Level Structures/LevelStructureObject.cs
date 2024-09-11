using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Level Structre", menuName = "ScriptableObjects/Level Structure Object")]
public class LevelStructureObject : ScriptableObject
{
    public float gridZCellSize;
    public StructureTile[] structureTiles;

    public LevelStructureObject(StructureTile[] structureTiles, int gridZCellSize)
    {
        this.structureTiles = structureTiles;
        this.gridZCellSize = gridZCellSize;
    }

    public void PlaceStructure(Vector3Int position, Tilemap tilemap)
    {
        float tilemapGridZCellSize = tilemap.GetComponentInParent<Grid>().cellSize.z;
        float zVal = gridZCellSize / tilemapGridZCellSize;

        if (gridZCellSize % tilemapGridZCellSize != 0 && tilemapGridZCellSize % gridZCellSize != 0) Debug.LogWarning($"The grid z cell size of the structure '{name}' is not divisable by the grid z cell size of the tilemap '{tilemap.name}.' This may cause issues with the structure's placement.");

        // Place the structure tiles on the tilemap
        foreach (StructureTile structureTile in structureTiles)
        {
            tilemap.SetTile(position + new Vector3Int(structureTile.position.x, structureTile.position.y, (structureTile.position.z * zVal).ConvertTo<int>()), structureTile.tile);
        }
    }

    [Serializable]
    public struct StructureTile
    {
        public Vector3Int position;
        public Tile tile;

        public StructureTile(Vector3Int position, Tile tile)
        {
            this.position = position;
            this.tile = tile;
        }
    }
}
