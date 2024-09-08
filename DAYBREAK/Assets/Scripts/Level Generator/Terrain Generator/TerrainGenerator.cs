using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] 
    public TerrainDataObject terrainDataObject;

    [SerializeField] 
    DebugOptions debugOptions;

    [SerializeField]
    private Material terrainMaterial;

    // Start is called before the first frame update
    void Start()
    {
        // Generate the terrain on start if the debug option is enabled
        if (debugOptions.generateOnStart) GenerateTerrain();
    }

    // Set the terrain data object
    public void SetTerrainData(TerrainDataObject terrainDataObject) => this.terrainDataObject = terrainDataObject;

    // Generate the terrain
    [ContextMenu("Generate Terrain")]
    public void GenerateTerrain()
    {
        if (!GenerationPreChecks()) return;

        // Clear the terrain
        ClearTerrain();

        // Create the terrain object
        Terrain terrain = CreateTerrainObject();

        // Generate the height map
        TerrainMap terrainMap = GenerateHeightMap(terrainDataObject.mapSize, terrainDataObject, terrainDataObject.seed);

        // Apply the terrain heights and transforms
        ApplyTerrainHeightsAndTrans(terrain, terrainMap);

        //Apply the terrain layers
        ApplyTerrainLayers(terrain, terrainMap);
    }

    private bool GenerationPreChecks()
    {
        Debug.ClearDeveloperConsole();

        // Check if the terrain data object is set
        if (!terrainDataObject)
        {
            Debug.LogError("Terrain Data Object is not set.");
            return false;
        }
        else if (terrainDataObject.biomes.Length <= 0)
        {
            Debug.LogError("No biomes are set in the Terrain Data Object.");
            return false;
        }

        // Checks complete
        return true;
    }

    // Clear the terrain
    [ContextMenu("Clear Terrain")]
    public void ClearTerrain()
    {
        foreach (Transform child in transform) DestroyImmediate(child.gameObject);
    }

    private Terrain CreateTerrainObject()
    {
        GameObject newInstance = new GameObject("Terrain");
        newInstance.transform.parent = transform;

        // Create the terrain data
        TerrainData terrainData = new TerrainData();

        // Set the terrain data properties
        Terrain newTerrain = newInstance.AddComponent<Terrain>();
        newTerrain.materialTemplate = terrainMaterial;

        TerrainCollider terrainCollider = newInstance.AddComponent<TerrainCollider>();

        // Set the terrain data properties
        newTerrain.terrainData = terrainData;
        terrainCollider.terrainData = terrainData;

        // Set the terrain size
        return newTerrain;
    }

    private void ApplyTerrainHeightsAndTrans(Terrain terrain, TerrainMap terrainMap)
    {
        terrain.terrainData.heightmapResolution = terrainMap.MapSize.x + 1;
        terrain.terrainData.size = new Vector3(terrainMap.MapSize.x, terrainMap.maxHeight, terrainMap.MapSize.y);

        terrain.terrainData.SetDetailResolution(terrainMap.MapSize.x, 32);

        // Set the terrain heights
        int terrainOffset = terrain.terrainData.heightmapResolution / 2 - terrainMap.MapSize.x / 2;
        terrain.terrainData.SetHeights(terrainOffset, terrainOffset, terrainMap.heights);

        if (debugOptions.centerGeneratedTerrain)
            terrain.gameObject.transform.position = new Vector3(-terrain.terrainData.heightmapResolution / 2, 0, -terrain.terrainData.heightmapResolution / 2);
    }

    private void ApplyTerrainLayers(Terrain terrain, TerrainMap terrainMap)
    {
        List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

        foreach (Biome biome in terrainDataObject.biomes)
        {
            if (biome.skipBiome) continue;

            if (!biome.baseTerrainLayer) Debug.LogError("Base Terrain Layer not set for biome: " + biome.name);
            else terrainLayers.Add(biome.baseTerrainLayer);
            
            if (!biome.pathTerrainLayer) Debug.LogError("Path Terrain Layer not set for biome: " + biome.name);
            else terrainLayers.Add(biome.pathTerrainLayer);
        }

        terrain.terrainData.terrainLayers = terrainLayers.ToArray();

        float[,,] map = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, terrain.terrainData.alphamapLayers];

        for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
            {
                for (int i = 0; i < terrain.terrainData.alphamapLayers; i++)
                {
                    int xIndex = (int)(x / (float)terrain.terrainData.alphamapWidth * terrainMap.MapSize.x);
                    int yIndex = (int)(y / (float)terrain.terrainData.alphamapHeight * terrainMap.MapSize.y);
                    int biomeIndex = i / 2;

                    map[x, y, i] = terrainMap.biomes[xIndex, yIndex, biomeIndex];
                }
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, map);
    }

    #region structs

    private struct TerrainMap
    {
        public float[,] heights;
        public float maxHeight;

        public float[,,] biomes;

        public TerrainMap(float[,] heights, float maxHeight, float[,,] biomes)
        {
            this.heights = heights;
            this.maxHeight = maxHeight;

            this.biomes = biomes;
        }

        public Vector2Int MapSize => new Vector2Int(heights.GetLength(0), heights.GetLength(1));
    }

    private TerrainMap GenerateHeightMap(Vector2Int mapSize, TerrainDataObject terrainDataObject, string seed)
    {
        int intseed = debugOptions.randomSeed ? System.DateTime.UtcNow.Millisecond : seed.GetHashCode();
        Random.InitState(intseed);

        Vector4 offset = new Vector4(Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000));

        float[,] heightMap = new float[mapSize.x, mapSize.y];
        float[,,] biomeMap = new float[mapSize.x, mapSize.y, terrainDataObject.biomes.Length];

        float maxHeight = 0;
        float minHeight = 0;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                List<float> terrainHeights = new List<float>();
                List<float> terrainSelectors = new List<float>();

                foreach (Biome biome in terrainDataObject.biomes)
                {
                    if (biome.skipBiome) continue;

                    float tempHeight = 0;
                    float tempSelector = 0;

                    foreach (AmpsAndFreq af in biome.terrainAF)
                        tempHeight += Mathf.PerlinNoise((x + offset.x) * af.frequency, (y + offset.y) * af.frequency) * af.amplitude;
                    terrainHeights.Add(tempHeight / biome.terrainAF.Length);

                    foreach (AmpsAndFreq af in biome.selectorAF)
                        tempSelector += Mathf.PerlinNoise((x + offset.z) * af.frequency, (y + offset.w) * af.frequency) * af.amplitude;
                    terrainSelectors.Add(tempSelector / biome.selectorAF.Length);

                    biomeMap[x, y, Array.IndexOf(terrainDataObject.biomes, biome)] = tempSelector;
                }

                float height = 0;
                for (int i = 0; i < terrainHeights.Count; i++)
                    height += terrainHeights[i] * (terrainSelectors[i] * terrainDataObject.biomeSeperation);

                height /= (terrainSelectors.Sum() * terrainDataObject.biomeSeperation);

                // apply the fall off curve to the terrain
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(mapSize.x / 2, mapSize.y / 2));
                height = Mathf.Lerp(height, 0, terrainDataObject.edgeCurve.Evaluate(Mathf.Clamp(dist - terrainDataObject.islandRadius - Mathf.PerlinNoise((x + offset.x) / terrainDataObject.naturalEdgeScale, (y + offset.y) / terrainDataObject.naturalEdgeScale) * terrainDataObject.naturalEdgeStrength, 0, Mathf.Infinity) / terrainDataObject.edgeStrength));

                if (height > maxHeight) maxHeight = height;
                if (height < minHeight) minHeight = height;

                heightMap[x, y] = height;
            }
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                heightMap[x, y] -= minHeight;
                heightMap[x, y] /= maxHeight - minHeight;
            }
        }

        return new TerrainMap(heightMap, maxHeight -= minHeight, biomeMap);
    }

    [Serializable]
    struct DebugOptions
    {
        public bool generateOnStart;
        public bool centerGeneratedTerrain;
        public bool randomSeed;
    }

    [Serializable]
    public struct Biome
    {
        public bool skipBiome;
        public string name;

        [Space]
        public TerrainLayer baseTerrainLayer;
        public TerrainLayer pathTerrainLayer;

        [Space]
        public AmpsAndFreq[] terrainAF;
        public AmpsAndFreq[] selectorAF;
    }

    [Serializable]
    public struct AmpsAndFreq
    {
        public float amplitude;
        public float frequency;
    }

    [Serializable]
    public struct TerrainMaterials
    {
        public Material top;
        public Material steep;
        public Material path;
    }

    #endregion
}

