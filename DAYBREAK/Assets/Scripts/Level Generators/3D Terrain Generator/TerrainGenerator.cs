using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Random = UnityEngine.Random;
using Path = TerrainPaths.Path;
using System.IO;

public class TerrainGenerator : MonoBehaviour
{
    Terrain _terrain;
    TerrainMap _terrainMap;
    public string _seed;

    [SerializeField] public TerrainDataObject terrainDataObject;

    [SerializeField] public DebugOptions debugOptions;

    [SerializeField] private Material terrainMaterial;

    private Dictionary<Vector2Int, float> _structurePosistionsAndRadii = new Dictionary<Vector2Int, float>();
    private List<Path> _paths = new List<Path>();

    // Start is called before the first frame update
    void Start()
    {
        // Generate the terrain on start if the debug option is enabled
        if (debugOptions.generateOnStart) GenerateTerrain();
    }

    // Set the terrain data object
    public void SetTerrainData(TerrainDataObject terrainDataObject) => this.terrainDataObject = terrainDataObject;

    // Generate the terrain
    public void GenerateTerrain()
    {
        if (!GenerationPreChecks()) return;

        // Clear the terrain
        ClearTerrain();

        // Create the terrain object
        _terrain = CreateNewTerrain(terrainDataObject);

        // Set the seed. CRY CONSTANTINE! COPE!
        int newSeed;
        if (debugOptions.randomSeed)
        {
            newSeed = System.DateTime.UtcNow.TimeOfDay.GetHashCode();
            _seed = newSeed.ToString();
        }
        else if (debugOptions.useDataObjectSeed)
        {
            newSeed = int.TryParse(terrainDataObject.seed, out int intSeed) ? intSeed : terrainDataObject.seed.GetHashCode();
            _seed = terrainDataObject.seed;
        }
        else
        {
            newSeed = int.TryParse(_seed, out int intSeed) ? intSeed : _seed.GetHashCode();
        }

        //Generate the terrain map
        _terrainMap = CreateNewTerrainMap(_terrain, terrainDataObject, newSeed);

        // Apply the terrain heights and transforms
        ApplyTerrainMap(_terrain, terrainDataObject, _terrainMap);

        // Generate the structures
        _structurePosistionsAndRadii = GenerateStructures(_terrain, terrainDataObject, _terrainMap);

        // Create the paths
        CreatePaths();

        //Temp
        FindObjectOfType<PlayerBase>().transform.position = GetNearestSpawnPos(Vector3.zero) + Vector3.up;
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

        // Set terrain object layer to Terrain
        newTerrain.gameObject.layer = LayerMask.NameToLayer("Terrain");

        _structurePosistionsAndRadii = new Dictionary<Vector2Int, float>();
        _paths = new List<Path>();

        // Set the terrain size
        return newTerrain;
    }

    private TerrainMap CreateNewTerrainMap(Terrain terrain, TerrainDataObject data, int seed)
    {
        // Get the biomes that are not set to be skipped
        Biome[] biomes = data.biomes.Where(b => !b.skipBiome).ToArray();

        // Log an error and return if no biomes are set to be generated
        if (biomes.Length <= 0) throw new Exception("No biomes are set to be generated.");

        // Set the random seed
        Random.InitState(seed);

        // Create an offset for every biome noise map
        Vector4[] mapOffsets = new Vector4[biomes.Length];
        for (int biomeIndex = 0; biomeIndex < biomes.Length; biomeIndex++) mapOffsets[biomeIndex] = new Vector4(Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000));

        // Create the weight map
        float[,,] weightMap = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, biomes.Length];

        // Generate the biome weight map
        for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
            for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
            {
                // Generate a weight for each biome
                for (int biomeIndex = 0; biomeIndex < biomes.Length; biomeIndex++)
                {
                    float weight = 0;

                    foreach (AmpsAndFreq af in biomes[biomeIndex].selectorAF)
                        weight += Mathf.Pow(Mathf.PerlinNoise((x + mapOffsets[biomeIndex].z) * af.frequency, (y + mapOffsets[biomeIndex].w) * af.frequency) * af.amplitude, data.biomeSeperation);

                    weightMap[x, y, biomeIndex] = weight / biomes[biomeIndex].selectorAF.Length;
                }

                // Normalize the weights
                float sum = 0;
                for (int biomeIndex = 0; biomeIndex < biomes.Length; biomeIndex++)
                    sum += weightMap[x, y, biomeIndex];

                for (int biomeIndex = 0; biomeIndex < biomes.Length; biomeIndex++)
                    weightMap[x, y, biomeIndex] /= sum;
            }

        // Create the height map
        float[,] heightMap = new float[data.mapSize.x, data.mapSize.y];
        float maxHeight = 0, minHeight = 0;

        // Generate the biome height map
        for (int x = 0; x < data.mapSize.x; x++)
            for (int y = 0; y < data.mapSize.y; y++)
            {
                float height = 0;

                // generate a height foreach biome
                for (int biomeIndex = 0; biomeIndex < biomes.Length; biomeIndex++)
                    foreach (AmpsAndFreq af in biomes[biomeIndex].terrainAF)
                    {
                        float weight = weightMap[x * weightMap.GetLength(0) / data.mapSize.x, y * weightMap.GetLength(1) / data.mapSize.y, biomeIndex];
                        height += Mathf.PerlinNoise((x + mapOffsets[biomeIndex].x) * af.frequency, (y + mapOffsets[biomeIndex].y) * af.frequency) * af.amplitude * weight;
                    }

                height /= biomes.Length;

                // apply the fall off curve to the terrain
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(data.mapSize.x / 2, data.mapSize.y / 2));
                height = Mathf.Lerp(height, 0, terrainDataObject.edgeCurve.Evaluate(Mathf.Clamp(dist - terrainDataObject.islandRadius - Mathf.PerlinNoise((x + mapOffsets[0].x) / terrainDataObject.naturalEdgeScale, (y + mapOffsets[0].y) / terrainDataObject.naturalEdgeScale) * terrainDataObject.naturalEdgeStrength, 0, Mathf.Infinity) / terrainDataObject.edgeStrength));
                
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

        // return the terrain map
        return new TerrainMap(heightMap, maxHeight - minHeight, biomes, weightMap);
    }

    private void ApplyTerrainMap(Terrain terrain, TerrainDataObject data, TerrainMap map)
    {
        // Set the terrain heights
        terrain.terrainData.size = new Vector3(map.heightMapSize.x, map.maxHeight, map.heightMapSize.y);
        int terrainOffset = terrain.terrainData.heightmapResolution / 2 - map.heightMapSize.x / 2;
        terrain.terrainData.SetHeights(terrainOffset, terrainOffset, map.heightMap);

        if (debugOptions.centerGeneratedTerrain) terrain.gameObject.transform.position = new Vector3(-terrain.terrainData.size.x / 2, 0, -terrain.terrainData.size.z / 2);

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
                for (int i = 0; i < map.biomes.Length; i++)
                {
                    layerAlphaMap[x, y, i] = _terrainMap.weightMap[x, y, i];
                }

        _terrain.terrainData.SetAlphamaps(0, 0, layerAlphaMap);
    }

    private Dictionary<Vector2Int, float> GenerateStructures(Terrain terrain, TerrainDataObject data, TerrainMap map)
    {
        Dictionary<Vector2Int, float> structurePosistionsAndRadii = new Dictionary<Vector2Int, float>();
        int structureCount = Mathf.RoundToInt(data.mapSize.x * data.mapSize.y * data.structureDensity * 0.0001f);

        // if the loop fails to generate a structure 10 times in a row, stop trying
        int structureFailures = 0;

        for (int i = 0; i < structureCount; i++)
        {
            if (structureFailures >= 10) break;

            // pick a random position on the map within the rough bounds of the island
            int xEdgeBuffer = Mathf.Clamp(data.mapSize.x / 2 - Mathf.RoundToInt(data.islandRadius), 0, data.mapSize.x);
            int yEdgeBuffer = Mathf.Clamp(data.mapSize.y / 2 - Mathf.RoundToInt(data.islandRadius), 0, data.mapSize.y);
            Vector2Int sMapLocal = new Vector2Int(Random.Range(xEdgeBuffer, data.mapSize.x - xEdgeBuffer), Random.Range(yEdgeBuffer, data.mapSize.y - yEdgeBuffer));

            Vector3 sPos = new Vector3(
                (float)sMapLocal.x / terrain.terrainData.heightmapResolution * data.mapSize.x,
                Mathf.Clamp(terrain.terrainData.GetHeight(Mathf.RoundToInt(sMapLocal.x), Mathf.RoundToInt(sMapLocal.y)), data.waterLevel, Mathf.Infinity),
                (float)sMapLocal.y / terrain.terrainData.heightmapResolution * data.mapSize.y
                );

            // fail if the structure is in the water
            if (sPos.y < data.waterLevel + data.structureElevationAboveWater)
            {
                structureFailures++;
                structureCount--;
                continue;
            }

            Biome dominantBiome = new Biome();
            float dominantBiomeWeight = 0;
            foreach (Biome biome in map.biomes)
            {
                float weight = _terrainMap.weightMap[Mathf.RoundToInt(sMapLocal.x), Mathf.RoundToInt(sMapLocal.y), Array.IndexOf(map.biomes, biome)];

                if (dominantBiomeWeight < weight)
                {
                    dominantBiomeWeight = weight;
                    dominantBiome = biome;
                }
            }

            // fail if biome has no structures
            if (dominantBiome.structures.Length <= 0) 
            { 
                structureFailures++;
                structureCount--;
                continue;
            }

            // Create the structure
            Structure structure = dominantBiome.structures[Random.Range(0, dominantBiome.structures.Length)];

            // fail if the structure is too close to another structure
            bool structureGeneratedInsideAnotherStructure = false;
            foreach (KeyValuePair<Vector2Int, float> kv in structurePosistionsAndRadii)
            {
                if (Vector2.Distance(sMapLocal, kv.Key) < kv.Value + structure.radius + terrainDataObject.structureSeperationBuffer)
                {
                    structureGeneratedInsideAnotherStructure = true;
                    break;
                }
            }

            if (structureGeneratedInsideAnotherStructure)
            {
                structureFailures++;
                structureCount--;
                continue;
            }

            // flaten the terrain around the structure
            int perimeter = (structure.radius + data.structureEdgeBuffer) * 2;

            // get the height map around the structure that is within the terrain bounds
            float[,] heightMap = terrain.terrainData.GetHeights(
                Mathf.Clamp(sMapLocal.x - perimeter / 2, 0, data.mapSize.x),
                Mathf.Clamp(sMapLocal.y - perimeter / 2, 0, data.mapSize.y),
                perimeter - (sMapLocal.x < 0 ? Mathf.Abs(sMapLocal.x) : 0) - (sMapLocal.x > (data.mapSize.x - perimeter / 2) ? sMapLocal.x - (data.mapSize.x - perimeter / 2) : 0),
                perimeter - (sMapLocal.y < 0 ? Mathf.Abs(sMapLocal.y) : 0) - (sMapLocal.y > (data.mapSize.y - perimeter / 2) ? sMapLocal.y - (data.mapSize.y - perimeter / 2) : 0));

            // get the flat terrain height
            float flatTerrain = heightMap[perimeter / 2, perimeter / 2];

            // apply the flat and structure edge curve to the terrain
            for (int x = 0; x < heightMap.GetLength(0); x++)
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(perimeter / 2, perimeter / 2));
                    float t = Mathf.Clamp(dist - structure.radius, 0, data.structureEdgeBuffer) / data.structureEdgeBuffer;
                    heightMap[x, y] = Mathf.Lerp(flatTerrain, heightMap[x, y], terrainDataObject.structureEdgeCurve.Evaluate(t));
                }

            terrain.terrainData.SetHeights(
                Mathf.Clamp(sMapLocal.x - perimeter / 2, 0, data.mapSize.x),
                Mathf.Clamp(sMapLocal.y - perimeter / 2, 0, data.mapSize.y),
                heightMap);

            GameObject structureObj = Instantiate(structure.obj);
            structureObj.transform.parent = terrain.transform;
            structureObj.transform.localPosition = sPos + new Vector3(0, structure.offestY, 0);
            structureObj.transform.rotation = Quaternion.Euler(0, 0, 0);

            structurePosistionsAndRadii.Add(sMapLocal, structure.radius);
        }

        return structurePosistionsAndRadii;
    }

    private void CreatePaths()
    {
        // Get the structures
        Dictionary<Vector2Int, float> structures = new Dictionary<Vector2Int, float>(_structurePosistionsAndRadii);

        List<Vector3> points = new List<Vector3>();

        // Get the start and end structures
        KeyValuePair<Vector2Int, float> start = structures.ElementAt(Random.Range(0, structures.Count));
        KeyValuePair<Vector2Int, float> end = structures.ElementAt(Random.Range(0, structures.Count));

        // Add the center point of a structure
        points.Add(new Vector3(start.Key.x, start.Key.y));

        // Add a random point on the boarder of the structure
        Vector3 dir = (new Vector3(end.Key.x, end.Key.y) - new Vector3(start.Key.x, start.Key.y)).normalized;
        float rotation = Random.Range(-45, 45);
        dir = Quaternion.Euler(0, 0, rotation) * dir;

        Vector3 startPoint = new Vector3(start.Key.x, start.Key.y) + dir * start.Value;
        startPoint = new Vector3(startPoint.x, startPoint.y);
        points.Add(startPoint);

        // Add a random point on the boarder of the second structure
        dir = (new Vector3(start.Key.x, start.Key.y) - new Vector3(end.Key.x, end.Key.y)).normalized;
        rotation = Random.Range(-45, 45);
        dir = Quaternion.Euler(0, 0, rotation) * dir;

        Vector3 endPoint = new Vector3(end.Key.x, end.Key.y) + dir * end.Value;
        endPoint = new Vector3(endPoint.x, endPoint.y);
        points.Add(endPoint);

        // Add the center point of the second structure
        points.Add(new Vector3(end.Key.x, end.Key.y));

        // Create the path
        _paths.Add(new Path(points.ToArray()));
    }

    public Vector3 GetNearestSpawnPos(Vector3 vector3)
    {
        // Get the terrain if it is not set
        if (!_terrain) _terrain = GetComponentInChildren<Terrain>();

        // Get the world to terrain local position
        Vector3 worldToTerrainLocal = _terrain.transform.InverseTransformPoint(vector3);

        // Clamp the position to the terrain bounds
        float x = Mathf.Clamp(worldToTerrainLocal.x, 0, _terrain.terrainData.size.x);
        float z = Mathf.Clamp(worldToTerrainLocal.z, 0, _terrain.terrainData.size.z);

        // Get the height at the clamped position
        Vector3Int vec = WorldToTerrain(new Vector3(x, 0, z));
        float y = _terrain.terrainData.GetHeight(vec.x, vec.z);

        // Return the nearest spawn position
        return new Vector3(x, y + 1f, z) + _terrain.transform.position;
    }

    // Convert world position to terrain position
    public Vector3Int WorldToTerrain(Vector3 vec)
    {
        int x = Mathf.RoundToInt(vec.x / terrainDataObject.mapSize.x * _terrain.terrainData.heightmapResolution);
        int y = Mathf.RoundToInt(vec.y / terrainDataObject.mapSize.y * _terrain.terrainData.heightmapResolution);
        int z = Mathf.RoundToInt(vec.z * _terrain.terrainData.heightmapResolution);

        return new Vector3Int(x, y, z);
    }

    // Convert terrain position to world position
    public Vector3 TerrainToWorld(Vector3 vec)
    {
        float x = vec.x / _terrain.terrainData.heightmapResolution * terrainDataObject.mapSize.x;
        float y = vec.y / _terrain.terrainData.heightmapResolution * terrainDataObject.mapSize.y;
        float z = vec.z / _terrain.terrainData.heightmapResolution;

        return new Vector3(x, y, z) - (new Vector3(terrainDataObject.mapSize.x, terrainDataObject.mapSize.y) / 2);
    }

    private void OnDrawGizmos()
    {
        if (!_terrain || !terrainDataObject) return;

        // map bounds
        Handles.color = Color.red;
        Handles.DrawWireDisc(_terrain.gameObject.transform.position + new Vector3(terrainDataObject.mapSize.x, 0, terrainDataObject.mapSize.y) / 2, Vector3.up, terrainDataObject.islandRadius);

        // structure bounds
        Handles.color = Color.green;
        foreach (KeyValuePair<Vector2Int, float> kv in _structurePosistionsAndRadii)
        {
            Vector3 pos = TerrainToWorld(new Vector3Int(kv.Key.x, kv.Key.y));
            Handles.DrawWireDisc(new Vector3(pos.x, 0, pos.y), Vector3.up, kv.Value);
        }

        // draw the paths
        foreach (Path path in _paths)
        {
            Handles.color = Color.cyan;
            foreach (Vector3 point in path.points)
            {
                Vector3 discPoint = TerrainToWorld(point);
                Handles.DrawWireDisc(new Vector3(discPoint.x, 0, discPoint.y), Vector3.up, 0.5f);
            }

            Handles.color = Color.blue;
            float detail = 1f / 10f;

            Vector3 lastPos = path.Start;

            for (float t = detail; t <= 1.1f; t += detail)
            {
                Vector3 newPos = path.Spline(t);

                Vector3 lastPoint = TerrainToWorld(lastPos);
                Vector3 newPoint = TerrainToWorld(newPos);

                Handles.DrawLine(new Vector3(lastPoint.x, 0, lastPoint.y), new Vector3(newPoint.x, 0, newPoint.y));

                lastPos = newPos;
            }
        }
    }

    #region structs

    private struct TerrainMap
    {
        public float[,] heightMap;
        public float maxHeight;

        public Biome[] biomes;
        public float[,,] weightMap;

        public TerrainMap(float[,] heightMap, float maxHeight, Biome[] biomes, float[,,] weightMap)
        {
            this.heightMap = heightMap;
            this.maxHeight = maxHeight;

            this.biomes = biomes;
            this.weightMap = weightMap;
        }

        public Vector2Int heightMapSize => new Vector2Int(heightMap.GetLength(0), heightMap.GetLength(1));

        public Vector2Int weightMapSize => new Vector2Int(weightMap.GetLength(0), weightMap.GetLength(1));
    }

    [Serializable] public struct DebugOptions
    {
        public bool generateOnStart;
        public bool centerGeneratedTerrain;
        public bool randomSeed;
        public bool useDataObjectSeed;
    }

    [Serializable] public struct Biome
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
        public Structure[] structures;
    }

    [Serializable] public struct AmpsAndFreq
    {
        public float amplitude;
        public float frequency;
    }

    [Serializable] public struct Structure
    {
        public GameObject obj;
        public float offestY;
        public int radius;
    }

    #endregion
}

