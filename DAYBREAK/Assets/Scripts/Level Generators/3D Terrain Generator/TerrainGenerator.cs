using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Random = UnityEngine.Random;

using Path = TerrainPaths.Path;
using BiomeData = TG_BiomeDataObject.BiomeData;
using Unity.VisualScripting;
using static TerrainPaths;
using UnityEngine.Rendering.Universal;

public class TerrainGenerator : MonoBehaviour
{
    Terrain _terrain;
    TerrainMap _terrainMap;
    public string _seed;

    [SerializeField] public TG_TerrainDataObject terrainDataObject;

    [SerializeField] public DebugOptions debugOptions;

    [SerializeField] private Material terrainMaterial;
    [SerializeField] private Material waterMaterial;

    private Dictionary<Vector2Int, float> _structurePosistionsAndRadii = new Dictionary<Vector2Int, float>();

    private Path[] _paths;

    // Start is called before the first frame update
    void Start()
    {
        // Generate the terrain on start if the debug option is enabled
        if (debugOptions.generateOnStart) GenerateTerrain();
    }

    // Set the terrain data object
    public void SetTerrainData(TG_TerrainDataObject terrainDataObject) => this.terrainDataObject = terrainDataObject;

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
        _paths = CreatePaths();

        //PaintPaths(_paths);

        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children) child.tag = "Terrain";
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

    private Terrain CreateNewTerrain(TG_TerrainDataObject data)
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

        // Set the terrain size
        return newTerrain;
    }

    private TerrainMap CreateNewTerrainMap(Terrain terrain, TG_TerrainDataObject data, int seed)
    {
        // Get the biomes that are not set to be skipped
        BiomeData[] biomes = new BiomeData[data.biomes.Length];
        for (int i = 0; i < data.biomes.Length; i++) biomes[i] = new BiomeData(data.biomes[i]);

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
                        weight += Mathf.PerlinNoise((x + mapOffsets[biomeIndex].z) * af.frequency, (y + mapOffsets[biomeIndex].w) * af.frequency) * af.amplitude;

                    weightMap[x, y, biomeIndex] = weight / biomes[biomeIndex].selectorAF.Length;
                }
            }

        weightMap = Float3Powerize(weightMap, data.biomeSeperation);

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
                height = Mathf.Lerp(height, 0, terrainDataObject.islandSlope.Evaluate(Mathf.Clamp(dist - terrainDataObject.islandSize - Mathf.PerlinNoise((x + mapOffsets[0].x) / terrainDataObject.islandBorderRoughness, (y + mapOffsets[0].y) / terrainDataObject.islandBorderRoughness) * terrainDataObject.islandBorderRoughnessStrength, 0, Mathf.Infinity) / terrainDataObject.islandSlopeStrength));

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

    private void ApplyTerrainMap(Terrain terrain, TG_TerrainDataObject data, TerrainMap map)
    {
        // Set the terrain heights
        terrain.terrainData.size = new Vector3(map.heightMapSize.x, map.maxHeight, map.heightMapSize.y);
        int terrainOffset = terrain.terrainData.heightmapResolution / 2 - map.heightMapSize.x / 2;
        terrain.terrainData.SetHeights(terrainOffset, terrainOffset, map.heightMap);

        if (debugOptions.centerGeneratedTerrain) terrain.gameObject.transform.position = new Vector3(-terrain.terrainData.size.x / 2, 0, -terrain.terrainData.size.z / 2);

        //Paint Terrain Biomes
        List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

        BiomeData[] biomes = new BiomeData[terrainDataObject.biomes.Length];
        for (int i = 0; i < terrainDataObject.biomes.Length; i++) biomes[i] = new BiomeData(terrainDataObject.biomes[i]);

        for (int i = 0; i < biomes.Length; i++)
        {
            if (!biomes[i].baseTerrainLayer) Debug.LogError("Base Terrain Layer not set for biome: " + terrainDataObject.biomes[i].name);
            else terrainLayers.Add(biomes[i].baseTerrainLayer);
        }

        _terrain.terrainData.terrainLayers = terrainLayers.ToArray();

        float[,,] layerAlphaMap = new float[_terrain.terrainData.alphamapWidth, _terrain.terrainData.alphamapHeight, _terrain.terrainData.alphamapLayers];

        float[,,] paintMap = Float3Powerize(map.weightMap, terrainDataObject.biomeSeperation);

        for (int x = 0; x < _terrain.terrainData.alphamapWidth; x++)
            for (int y = 0; y < _terrain.terrainData.alphamapHeight; y++)
                for (int i = 0; i < map.biomes.Length; i++)
                {
                    layerAlphaMap[x, y, i] = paintMap[x, y, i];
                }

        _terrain.terrainData.SetAlphamaps(0, 0, layerAlphaMap);
    }

    private Dictionary<Vector2Int, float> GenerateStructures(Terrain terrain, TG_TerrainDataObject data, TerrainMap map)
    {
        Dictionary<Vector2Int, float> structurePosistionsAndRadii = new Dictionary<Vector2Int, float>();
        int structureCount = Mathf.RoundToInt(data.mapSize.x * data.mapSize.y * data.structureDensity * 0.0001f);

        // if the loop fails to generate a structure 10 times in a row, stop trying
        int structureFailures = 0;

        for (int i = 0; i < structureCount; i++)
        {
            if (structureFailures >= 10) break;

            // pick a random position on the map within the rough bounds of the island
            int xEdgeBuffer = Mathf.Clamp(data.mapSize.x / 2 - Mathf.RoundToInt(data.islandSize), 0, data.mapSize.x);
            int yEdgeBuffer = Mathf.Clamp(data.mapSize.y / 2 - Mathf.RoundToInt(data.islandSize), 0, data.mapSize.y);
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

            BiomeData dominantBiome = new BiomeData();
            float dominantBiomeWeight = 0;

            for (int biome = 0; biome < map.biomes.Length; biome++)
            {
                float weight = _terrainMap.weightMap[
                    Mathf.Clamp(Mathf.RoundToInt(sMapLocal.x), 0, _terrainMap.weightMap.GetLength(0) - 1),
                    Mathf.Clamp(Mathf.RoundToInt(sMapLocal.y), 0, _terrainMap.weightMap.GetLength(1) - 1),
                    biome];

                if (dominantBiomeWeight < weight)
                {
                    dominantBiomeWeight = weight;
                    dominantBiome = map.biomes[biome];
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
            TG_StructureDataObject structure = dominantBiome.structures[Random.Range(0, dominantBiome.structures.Length)];

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

    private Path[] CreatePaths()
    {
        List<Path> paths = new List<Path>();

        // Get the structures
        Dictionary<Vector2Int, float> tempStructures = new Dictionary<Vector2Int, float>(_structurePosistionsAndRadii);

        int structureReset = 0;

        while (tempStructures.Count > 0)
        {
            // Get the start and end structures
            List<Vector3> points = new List<Vector3>();

            KeyValuePair<Vector2Int, float> originStructure = tempStructures.ElementAt(Random.Range(0, tempStructures.Count));
            tempStructures.Remove(originStructure.Key);

            KeyValuePair<Vector2Int, float> destinationStructure = _structurePosistionsAndRadii.ElementAt(Random.Range(0, tempStructures.Count));

            foreach (KeyValuePair<Vector2Int, float> structure in _structurePosistionsAndRadii)
            {
                if (structure.Key == originStructure.Key || structure.Key == destinationStructure.Key) continue;

                // check if the path intercects with another structure
                Vector2 vec = destinationStructure.Key - originStructure.Key;
                Vector2 structurePos = structure.Key - originStructure.Key;

                // check if the structure intersects with the path between the start and end structures
                bool radiusIntersects = Mathf.Abs(Mathf.Sin(Vector2.SignedAngle(vec, structurePos) * Mathf.Deg2Rad) * structurePos.magnitude) <= structure.Value;
                bool distanceIntersects = structurePos.magnitude < vec.magnitude;
                if (radiusIntersects && distanceIntersects)
                {
                    structureReset++;
                    destinationStructure = structure;
                }
            }

            TG_PathDataObject[] originPaths = _terrainMap.GetDominantBiome(originStructure.Key).pathData;
            TG_PathDataObject[] destinationPaths = _terrainMap.GetDominantBiome(destinationStructure.Key).pathData;

            TG_PathDataObject originPath = originPaths[Random.Range(0, originPaths.Length)];
            TG_PathDataObject destinationPath = destinationPaths[Random.Range(0, destinationPaths.Length)];

            Vector3 startPoint = GetStructureBoarderPos(originStructure, new Vector3(destinationStructure.Key.x, destinationStructure.Key.y) - new Vector3(originStructure.Key.x, originStructure.Key.y));
            Vector3 endPoint = GetStructureBoarderPos(destinationStructure, new Vector3(originStructure.Key.x, originStructure.Key.y) - new Vector3(destinationStructure.Key.x, destinationStructure.Key.y));

            Vector3 startControl = (new Vector3(originStructure.Key.x, originStructure.Key.y) - startPoint).normalized * originPath.controlPointPower + startPoint;
            Vector3 endControl = (new Vector3(destinationStructure.Key.x, destinationStructure.Key.y) - endPoint).normalized * destinationPath.controlPointPower + endPoint;

            // create the path
            Path tempPath = new Path(new Vector3[4] {startControl, startPoint, endPoint, endControl});

            foreach (Path path in paths)
            {
                IntersectionData intersectionData = TerrainPaths.PathToPathIntersection(tempPath, path, 0.1f);

                if (intersectionData.intersects)
                {
                    destinationPaths = _terrainMap.GetDominantBiome(intersectionData.intersection).pathData;
                    destinationPath = destinationPaths[Random.Range(0, destinationPaths.Length)];

                    tempPath.End = intersectionData.intersection;
                    tempPath.EndControl = intersectionData.intersection - intersectionData.normal * Vector2.Distance(tempPath.Start, intersectionData.intersection) * destinationPath.controlPointPower;
                }
            }

            tempPath.originData = originPath;
            tempPath.destinationData = destinationPath;

            tempStructures.Remove(destinationStructure.Key);

            paths.Add(tempPath);

            tempStructures.Remove(originStructure.Key);
            tempStructures.Remove(destinationStructure.Key);
        }

        return paths.ToArray();

        Vector3 GetStructureBoarderPos(KeyValuePair<Vector2Int, float> structure, Vector3 direction)
        {
            float rotation = Random.Range(-45, 45);
            direction = Quaternion.Euler(0, 0, rotation) * direction.normalized;

            Vector3 point = new Vector3(structure.Key.x, structure.Key.y) + direction * structure.Value;
            point = new Vector3(point.x, point.y);

            return point;
        }
    }

    private void PaintPaths(Path[] paths)
    {
        float[,] heights = _terrain.terrainData.GetHeights(0, 0, _terrain.terrainData.heightmapResolution, _terrain.terrainData.heightmapResolution);
        float[,] pathHeights = new float[_terrain.terrainData.heightmapResolution, _terrain.terrainData.heightmapResolution];

        for (int x = 0; x < heights.GetLength(0); x++)
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                Path nearestPath = new Path();

                Vector2 nearestPoint = new Vector2();
                Vector2 nextNearestPoint = new Vector2();

                float nearestPointDist = float.MaxValue;
                float nextNearestPointDist = float.MaxValue;

                float nearestPathHeight = 0;
                float nextNearestPathHeight = 0;

                float pathT = 0;

                foreach (Path path in paths)
                {
                    float t = 0;

                    while (t <= 1)
                    {
                        t = Mathf.Clamp(t, 0, 1);

                        Vector2 pathPos = path.Spline(t);
                        pathPos = new Vector2(pathPos.y, pathPos.x); // path spline needs to be flipped
                        float dist = Vector2.Distance(new Vector2(x, y), pathPos);

                        if (dist < nearestPointDist)
                        {
                            nextNearestPointDist = nearestPointDist;
                            nextNearestPoint = nearestPoint;

                            nearestPointDist = dist;
                            nearestPoint = pathPos;

                            nearestPath = path;

                            nextNearestPathHeight = nearestPathHeight;

                            int hX = Mathf.Clamp(Mathf.RoundToInt(pathPos.x), 0, heights.GetLength(0) - 1);
                            int hY = Mathf.Clamp(Mathf.RoundToInt(pathPos.y), 0, heights.GetLength(1) - 1);
                            nearestPathHeight = heights[hX, hY];
                            pathT = t;
                        }

                        if (t == 1) break;
                        else t = Mathf.Clamp(t + 0.1f, 0, 1);
                    }
                }

                BiomeData biome = _terrainMap.GetDominantBiome(nearestPoint);
                TG_PathDataObject pathData = biome.pathData.Contains(nearestPath.originData) ? nearestPath.originData : nearestPath.destinationData;

                Vector2 hyp = new Vector2(x, y) - nearestPoint;
                Vector2 adj = nextNearestPoint - nearestPoint;
                float angle = Vector2.Angle(hyp, adj) * Mathf.Deg2Rad;

                float heightBlend = hyp.magnitude * Mathf.Cos(angle) / adj.magnitude;
                float pathHeight = Mathf.Lerp(nearestPathHeight, nextNearestPathHeight, heightBlend);

                float blend = Mathf.Clamp01((nearestPointDist - pathData.pathWidth) / pathData.pathFade);
                pathHeights[x, y] = Mathf.Lerp(pathHeight, heights[x, y], blend);
            }

        _terrain.terrainData.SetHeights(0, 0, pathHeights);
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

    public Vector2Int WorldToTerrainV2(Vector3 vec)
    {
        int x = Mathf.RoundToInt(vec.x / terrainDataObject.mapSize.x * _terrain.terrainData.heightmapResolution);
        int y = Mathf.RoundToInt(vec.z / terrainDataObject.mapSize.y * _terrain.terrainData.heightmapResolution);
        return new Vector2Int(x, y);
    }

    // Convert terrain position to world position
    public Vector3 TerrainToWorld(Vector3 vec)
    {
        float x = vec.x / _terrain.terrainData.heightmapResolution * terrainDataObject.mapSize.x;
        float y = vec.y / _terrain.terrainData.heightmapResolution * terrainDataObject.mapSize.y;
        float z = vec.z / _terrain.terrainData.heightmapResolution;

        return new Vector3(x, y, z) - (new Vector3(terrainDataObject.mapSize.x, terrainDataObject.mapSize.y) / 2);
    }

    float[,,] Float3Powerize(float[,,] ar, float pow)
    {
        // Create a new array
        float[,,] newAR = new float[ar.GetLength(0), ar.GetLength(1), ar.GetLength(2)];

        // Loop through the array
        for (int x = 0; x < ar.GetLength(0); x++)
            for (int y = 0; y < ar.GetLength(1); y++)
            {
                // Multiply the values by the power
                for (int z = 0; z < ar.GetLength(2); z++)
                    newAR[x, y, z] = Mathf.Pow(ar[x, y, z], pow);

                // Normalize the values
                float sum = 0;

                // get sum of all values
                for (int i = 0; i < ar.GetLength(2); i++)
                    sum += newAR[x, y, i];

                // divide all values by the sum
                for (int i = 0; i < ar.GetLength(2); i++)
                    newAR[x, y, i] /= sum;
            }

        // return the new array
        return newAR;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_terrain || !terrainDataObject) return;

        // map bounds
        Handles.color = Color.red;
        Handles.DrawWireDisc(_terrain.gameObject.transform.position + new Vector3(terrainDataObject.mapSize.x, 0, terrainDataObject.mapSize.y) / 2, Vector3.up, terrainDataObject.islandSize);

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

            for (int i = 1; i < path.points.Length - 1; i++)
            {
                Vector3 discPoint = TerrainToWorld(path.points[i]);
                Handles.DrawWireDisc(new Vector3(discPoint.x, 0, discPoint.y), Vector3.up, 0.5f);
            }

            Handles.color = new Color(0.9f, 0.4f, 0);
            float detail = 0.1f;

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
#endif

    #region structs

    private struct TerrainMap
    {
        public float[,] heightMap;
        public float maxHeight;

        public BiomeData[] biomes;
        public float[,,] weightMap;

        public TerrainMap(float[,] heightMap, float maxHeight, BiomeData[] biomes, float[,,] weightMap)
        {
            this.heightMap = heightMap;
            this.maxHeight = maxHeight;

            this.biomes = biomes;
            this.weightMap = weightMap;
        }

        public Vector2Int heightMapSize => new Vector2Int(heightMap.GetLength(0), heightMap.GetLength(1));

        public Vector2Int weightMapSize => new Vector2Int(weightMap.GetLength(0), weightMap.GetLength(1));

        public BiomeData GetDominantBiome(Vector2Int pos)
        {
            float dominantBiomeWeight = 0;
            BiomeData dominantBiome = new BiomeData();

            for (int biome = 0; biome < biomes.Length; biome++)
            {
                int wX = Mathf.Clamp(pos.x, 0, weightMap.GetLength(0) - 1);
                int wY = Mathf.Clamp(pos.y, 0, weightMap.GetLength(1) - 1);

                float weight = weightMap[wX, wY, biome];

                if (weight > dominantBiomeWeight)
                {
                    dominantBiome = biomes[biome];
                    dominantBiomeWeight = weight;
                }
            }

            return dominantBiome;
        }

        public BiomeData GetDominantBiome(Vector3 pos) => GetDominantBiome(new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z)));

        public BiomeData GetDominantBiome(Vector2 pos) => GetDominantBiome(new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)));
    }

    [Serializable] public struct DebugOptions
    {
        public bool generateOnStart;
        public bool centerGeneratedTerrain;
        public bool randomSeed;
        public bool useDataObjectSeed;
    }

    [Serializable] public struct AmpsAndFreq
    {
        public float amplitude;
        public float frequency;
    }

    #endregion
}

