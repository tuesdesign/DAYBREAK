using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Biome = TerrainGenerator.Biome;

[CreateAssetMenu(fileName = "New Terrain Data Object", menuName = "ScriptableObjects/Terrain Data Object")]
public class TerrainDataObject : ScriptableObject
{
    [Space]
    public string seed;

    [Space]
    public Vector2Int mapSize;

    [Space] 
    public Biome[] biomes;
    public float biomeSeperation;

    [Space]
    [Tooltip("structureDensity of 1 will yield 1 structure per 100 x 100 map size")] public float structureDensity;
    public int structureEdgeBuffer;
    public AnimationCurve structureEdgeCurve;
    public float structureElevationAboveWater;

    [Space]
    public float waterLevel = 0.5f;

    [Space] 
    public float islandRadius = 0.5f;

    [Space] 
    public AnimationCurve edgeCurve;
    public float edgeStrength = 0.1f;

    [Space] 
    public float naturalEdgeScale = 0.1f;
    public float naturalEdgeStrength = 0.1f;
}
