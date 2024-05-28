using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelStructureCreator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile debugTile;

    public void PlaceTileAtPosition(Tilemap tilemap, Tile tile, Vector3Int pos)
    {
        if (!tilemap || !tile)
        {
            Debug.LogError("Tilemap or tile is null!");
            return;
        }

        tilemap.SetTile(pos, tile);
    }
}
