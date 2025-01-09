using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain Data Object", menuName = "3D Terrain Generator/Terrain Data Object")]
public class TG_TerrainDataObject : ScriptableObject
{
    [Space]
    public string seed;

    [Space]
    public Vector2Int mapSize;

    [Space] 
    public TG_BiomeDataObject[] biomes;
    public float biomeSeperation;
    public float biomePaintSeperation;

    [Space]
    [Tooltip("structureDensity of 1 will yield 1 structure per 100 x 100 map size")] public float structureDensity;
    public int structureEdgeBuffer;
    public AnimationCurve structureEdgeCurve;
    public float structureSeperationBuffer;
    public float structureElevationAboveWater;

    [Space]
    public float waterLevel = 0.5f;

    [Space] 
    public float islandSize = 0.5f;

    [Space] 
    public AnimationCurve islandSlope;
    public float islandSlopeStrength = 0.1f;

    [Space] 
    public float islandBorderRoughness = 0.1f;
    public float islandBorderRoughnessStrength = 0.1f;
}
