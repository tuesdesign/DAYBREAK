using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Biome = TerrainGenerator.Biome;

[CreateAssetMenu(fileName = "New Terrain Data Object", menuName = "ScriptableObjects/Terrain Data Object")]
public class TerrainDataObject : ScriptableObject
{
    public Vector2Int mapSize;
    public string seed;

    [Space] public Biome[] biomes;
    public float biomeSeperation;

    [Space] public float islandRadius = 0.5f;

    [Space] public AnimationCurve edgeCurve;
    public float edgeStrength = 0.1f;

    [Space] public float naturalEdgeScale = 0.1f;
    public float naturalEdgeStrength = 0.1f;

    [Space] public float waterLevel = 0.5f;
}
