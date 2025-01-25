using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome Data Object", menuName = "3D Terrain Generator/Path Data Object")]
public class TG_PathDataObject : ScriptableObject
{
#if UNITY_EDITOR
    public bool showColors;
#endif

    public TerrainLayer layer;
    public Color[] pathColors;
    public float pathWidth;
    public float pathFade;
    public float controlPointPower;
}
