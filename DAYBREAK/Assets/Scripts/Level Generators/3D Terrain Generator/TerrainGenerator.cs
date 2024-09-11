using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    Terrain _terrain;
    TerrainMap _terrainMap;

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
        _terrain = CreateNewTerrain(terrainDataObject);

        // Generate the height map
        int seed = debugOptions.randomSeed ? System.DateTime.UtcNow.Millisecond : terrainDataObject.seed.GetHashCode();
        _terrainMap = CreateNewTerrainMap(_terrain, terrainDataObject, seed);

        // Apply the terrain heights and transforms
        ApplyTerrainMap(_terrain, terrainDataObject, _terrainMap);

        // Generate the structures
        GenerateStructures();

        // Create the paths
        CreatePaths();
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

    public void ClearTerrain()
    {
        foreach (Transform child in transform) DestroyImmediate(child.gameObject);
    }

    private Terrain CreateNewTerrain(TerrainDataObject data)
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

        // Set the terrain data properties
        newTerrain.terrainData.heightmapResolution = data.mapSize.x + 1;

        // Set the terrain detail resolution
        newTerrain.terrainData.SetDetailResolution(data.mapSize.x, 32);

        // Set the terrain size
        return newTerrain;
    }

    private TerrainMap CreateNewTerrainMap(Terrain terrain, TerrainDataObject data, int seed)
    {
        #region biome checks

        bool noBiomes = true;
        foreach (Biome biome in data.biomes)
            if (biome.skipBiome) continue;
            else
            {
                noBiomes = false; break;
            }
        if (noBiomes)
        {
            Debug.LogError("No biomes are set to be generated.");
            return new TerrainMap(new float[data.mapSize.x, data.mapSize.y], 0, new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, data.biomes.Length]);
        }

        #endregion

        Random.InitState(seed);

        #region biome weight map initialization

        float[,,] biomeWeightMap = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, data.biomes.Length];

        // define biome weights
        foreach (Biome biome in data.biomes)
        {
            Vector4 biomeNoiseOffset = new Vector4(Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000));

            // skip biome if it is set to be skipped
            if (biome.skipBiome) continue;

            // generate biome weight map
            for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
                for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
                {
                    float weight = 0;

                    foreach (AmpsAndFreq af in biome.selectorAF)
                        weight += Mathf.Pow(Mathf.PerlinNoise((x + biomeNoiseOffset.z) * af.frequency, (y + biomeNoiseOffset.w) * af.frequency) * af.amplitude, data.biomeSeperation);

                    biomeWeightMap[x, y, Array.IndexOf(data.biomes, biome)] = weight;
                }
        }

        // normalize biome weight map
        for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
            for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
            {
                float sum = 0;
                for (int i = 0; i < data.biomes.Length; i++)
                    sum += biomeWeightMap[x, y, i];

                for (int i = 0; i < data.biomes.Length; i++)
                    biomeWeightMap[x, y, i] /= sum;
            }

        #endregion

        #region terrain height map initialization

        float[,] heightMap = new float[data.mapSize.x, data.mapSize.y];

        float maxHeight = 0;
        float minHeight = 0;

        Vector4 heightNoiseOffset = new Vector4(Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000));

        // generate terrain height map
        for (int x = 0; x < data.mapSize.x; x++)
            for (int y = 0; y < data.mapSize.y; y++)
            {
                float height = 0;

                foreach (Biome biome in data.biomes)
                {
                    // skip biome if it is set to be skipped
                    if (biome.skipBiome) continue;

                    // generate terrain height
                    foreach (AmpsAndFreq af in biome.terrainAF)
                    {
                        float weight = biomeWeightMap[x * biomeWeightMap.GetLength(0) / data.mapSize.x, y * biomeWeightMap.GetLength(1) / data.mapSize.y, Array.IndexOf(data.biomes, biome)];
                        height += Mathf.PerlinNoise((x + heightNoiseOffset.x) * af.frequency, (y + heightNoiseOffset.y) * af.frequency) * af.amplitude * weight;
                    }
                }

                height /= data.biomes.Length;

                // apply the fall off curve to the terrain
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(data.mapSize.x / 2, data.mapSize.y / 2));
                height = Mathf.Lerp(height, 0, terrainDataObject.edgeCurve.Evaluate(Mathf.Clamp(dist - terrainDataObject.islandRadius - Mathf.PerlinNoise((x + heightNoiseOffset.x) / terrainDataObject.naturalEdgeScale, (y + heightNoiseOffset.y) / terrainDataObject.naturalEdgeScale) * terrainDataObject.naturalEdgeStrength, 0, Mathf.Infinity) / terrainDataObject.edgeStrength));

                // update the max and min height
                if (height > maxHeight) maxHeight = height;
                if (height < minHeight) minHeight = height;

                // apply the height to the heightMap
                heightMap[x, y] = height;
            }

        // lower the terrain to the lowest point
        for (int x = 0; x < data.mapSize.x; x++)
        {
            for (int y = 0; y < data.mapSize.y; y++)
            {
                heightMap[x, y] -= minHeight;
                heightMap[x, y] /= maxHeight - minHeight;
            }
        }

        #endregion

        return new TerrainMap(heightMap, maxHeight - minHeight, biomeWeightMap);
    }

    private void ApplyTerrainMap(Terrain terrain, TerrainDataObject data, TerrainMap map)
    {
        // Set the terrain heights
        terrain.terrainData.size = new Vector3(map.size.x, map.maxHeight, map.size.y);
        int terrainOffset = terrain.terrainData.heightmapResolution / 2 - map.size.x / 2;
        terrain.terrainData.SetHeights(terrainOffset, terrainOffset, map.heights);

        if (debugOptions.centerGeneratedTerrain) terrain.gameObject.transform.position = new Vector3(-terrain.terrainData.heightmapResolution / 2, 0, -terrain.terrainData.heightmapResolution / 2);

        //Paint Terrain Biomes
        List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

        foreach (Biome biome in terrainDataObject.biomes)
        {
            if (biome.skipBiome) continue;

            if (!biome.baseTerrainLayer) Debug.LogError("Base Terrain Layer not set for biome: " + biome.name);
            else terrainLayers.Add(biome.baseTerrainLayer);
        }

        _terrain.terrainData.terrainLayers = terrainLayers.ToArray();

        float[,,] layerAlphaMap = new float[_terrain.terrainData.alphamapWidth, _terrain.terrainData.alphamapHeight, _terrain.terrainData.alphamapLayers];

        for (int x = 0; x < _terrain.terrainData.alphamapWidth; x++)
            for (int y = 0; y < _terrain.terrainData.alphamapHeight; y++)
                for (int i = 0; i < data.biomes.Length; i++)
                {
                    layerAlphaMap[x, y, i] = _terrainMap.biomeWeights[x, y, i];
                }

        _terrain.terrainData.SetAlphamaps(0, 0, layerAlphaMap);
    }

    private void GenerateStructures()
    {
        throw new NotImplementedException();
    }

    private void CreatePaths()
    {
        throw new NotImplementedException();
    }

    #region structs

    private struct TerrainMap
    {
        public float[,] heights;
        public float maxHeight;

        public float[,,] biomeWeights;

        public TerrainMap(float[,] heights, float maxHeight, float[,,] biomeWeights)
        {
            this.heights = heights;
            this.maxHeight = maxHeight;

            this.biomeWeights = biomeWeights;
        }

        public Vector2Int size => new Vector2Int(heights.GetLength(0), heights.GetLength(1));
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
        public string name;
        public bool skipBiome;

        [Space]
        public TerrainLayer baseTerrainLayer;
        public TerrainLayer pathTerrainLayer;

        [Space]
        public AmpsAndFreq[] terrainAF;
        public AmpsAndFreq[] selectorAF;

        [Space]
        public GameObject[] structures;
    }

    [Serializable]
    public struct AmpsAndFreq
    {
        public float amplitude;
        public float frequency;
    }

    #endregion
}

