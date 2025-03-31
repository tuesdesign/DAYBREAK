using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    [SerializeField] private bool OnStart = true;
    private TerrainGenerator _terrainGenerator;
    
    private void Start()
    {
        _terrainGenerator = FindObjectOfType<TerrainGenerator>();
        _terrainGenerator._seed = System.DateTime.Now.ToString("yyyyMMdd");
        if (OnStart) _terrainGenerator.GenerateTerrain();
    }
}
