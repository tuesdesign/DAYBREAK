using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Level Structre", menuName = "ScriptableObjects/Level Structure Object")]
public class LevelStructureObject : ScriptableObject
{
    public StructureTile[] structureTiles;

    public LevelStructureObject(StructureTile[] structureTiles)
    {
        this.structureTiles = structureTiles;
    }

    public void PlaceStructure(Vector3Int position, Tilemap tilemap)
    {
        // Place the structure tiles on the tilemap
        foreach (StructureTile structureTile in structureTiles)
        {
            tilemap.SetTile(position + structureTile.position, structureTile.tile);
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
