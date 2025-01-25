using UnityEngine;
using static TerrainGenerator;

[CreateAssetMenu(fileName = "Biome Data Object", menuName = "3D Terrain Generator/Biome Data Object")]
public class TG_BiomeDataObject : ScriptableObject
{
#if UNITY_EDITOR
    public bool showColors;

    public bool showAFs;
    public bool showTerrainAF;
    public bool showSelectorAF;

    public bool showStructures;
    public bool[] showStructure;

    public bool showPaths;
    public bool[] showPath;
#endif

    public TerrainLayer baseTerrainLayer;
    public Color[] biomeColors;

    public AmpsAndFreq[] terrainAF;
    public AmpsAndFreq[] selectorAF;

    public TG_StructureDataObject[] structures;
    public TG_PathDataObject[] paths;

    public struct BiomeData
    {
        public TerrainLayer baseTerrainLayer;
        public Color[] biomeColors;
        public AmpsAndFreq[] terrainAF;
        public AmpsAndFreq[] selectorAF;
        public TG_StructureDataObject[] structures;
        public TG_PathDataObject[] pathData;

        public BiomeData(TerrainLayer baseTerrainLayer, Color[] biomeColors, AmpsAndFreq[] terrainAF, AmpsAndFreq[] selectorAF, TG_StructureDataObject[] structures, TG_PathDataObject[] paths)
        {
            this.baseTerrainLayer = baseTerrainLayer;
            this.biomeColors = biomeColors;
            this.terrainAF = terrainAF;
            this.selectorAF = selectorAF;
            this.structures = structures;
            this.pathData = paths;
        }

        public BiomeData(TG_BiomeDataObject obj)
        {
            this.baseTerrainLayer = obj.baseTerrainLayer;
            this.biomeColors = obj.biomeColors;
            this.terrainAF = obj.terrainAF;
            this.selectorAF = obj.selectorAF;
            this.structures = obj.structures;
            this.pathData = obj.paths;
        }
    }
}
