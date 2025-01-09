using UnityEngine;
using static TerrainGenerator;

[CreateAssetMenu(fileName = "Biome Data Object", menuName = "3D Terrain Generator/Biome Data Object")]
public class TG_BiomeDataObject : ScriptableObject
{
#if UNITY_EDITOR
    public bool showAFs;
    public bool showTerrainAF;
    public bool showSelectorAF;

    public bool showStructures;
    public bool[] showStructure;
#endif

    public TerrainLayer baseTerrainLayer;
    public TerrainLayer pathTerrainLayer;

    public AmpsAndFreq[] terrainAF;
    public AmpsAndFreq[] selectorAF;

    public TG_StructureDataObject[] structures;

    public struct BiomeData
    {
        public TerrainLayer baseTerrainLayer;
        public TerrainLayer pathTerrainLayer;
        public AmpsAndFreq[] terrainAF;
        public AmpsAndFreq[] selectorAF;
        public TG_StructureDataObject[] structures;

        public BiomeData(TerrainLayer baseTerrainLayer, TerrainLayer pathTerrainLayer, AmpsAndFreq[] terrainAF, AmpsAndFreq[] selectorAF, TG_StructureDataObject[] structures)
        {
            this.baseTerrainLayer = baseTerrainLayer;
            this.pathTerrainLayer = pathTerrainLayer;
            this.terrainAF = terrainAF;
            this.selectorAF = selectorAF;
            this.structures = structures;
        }

        public BiomeData(TG_BiomeDataObject obj)
        {
            this.baseTerrainLayer = obj.baseTerrainLayer;
            this.pathTerrainLayer = obj.pathTerrainLayer;
            this.terrainAF = obj.terrainAF;
            this.selectorAF = obj.selectorAF;
            this.structures = obj.structures;
        }
    }
}
