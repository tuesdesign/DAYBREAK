using System;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public int _seed;
    [SerializeField] public string _currentLevelName;
    private int[,] _levelGrid;

    [Space, Header("Temp Inspector Tiles!"), SerializeField] public IsometricRuleTile groundTop;
    [SerializeField] public IsometricRuleTile groundFill;
    [SerializeField] public IsometricRuleTile water;
    [SerializeField] public IsometricRuleTile debugTile;

    [Space, SerializeField] LevelSettings[] _levelSettings;
    private LevelSettings _currentLS;

    [Space, SerializeField] public Tilemap tilemap;

    private void Awake()
    {
        if (int.TryParse(_currentLevelName, out int levelIndex) && levelIndex >= 0)
        {
            // if the level name is a number, generate the level with the given index
            if (_levelSettings.Length <= levelIndex)
            {
                Debug.LogError("Level index out of range! " +
                    ((_levelSettings.Length - 1 == 0) ?
                    "\nCurrently, the only acceptable index is 0!" :
                    $"\nCurrently, the acceptable range of valuse is between 0 and {_levelSettings.Length - 1}!"));
                return;
            }

            // Get the level name from the index
            string indexedLevelName = _levelSettings[levelIndex].levelName;
            GenerateLevel(indexedLevelName);
        }
        else if (_currentLevelName.Length > 0)
        {
            // if the level name is not empty, generate the level with the given name
            GenerateLevel(_currentLevelName);
        }
        else
        {
            // if the level name is empty, generate the first level in the list
            GenerateLevel();
        }
    }

    public bool GenerateLevel()
    {
        // if the level settings are empty, set the level setting to the first element in the list
        if (_currentLS.Equals(new LevelSettings())) _currentLS = _levelSettings[0];

        // set the seed for the random number generator
        UnityEngine.Random.InitState(_seed);
        float seedOffsetX = UnityEngine.Random.Range(-100000f, 100000f);
        float seedOffsetY = UnityEngine.Random.Range(-100000f, 100000f);

        // create the grid for the level
        _levelGrid = new int[_currentLS.levelSize.x, _currentLS.levelSize.y];

        // Generate the terrain
        for (int x = 0; x < _currentLS.levelSize.x; x++)
            for (int y = 0; y < _currentLS.levelSize.y; y++)
            {
                float zLevel = 0;

                // create the terrain using perlin noise, for each frequency and amplitude
                for (int i = 0; i < _currentLS.levelFrequencyAndAmplitude.frequecies.Length; i++)
                {
                    zLevel += _currentLS.levelFrequencyAndAmplitude.amplitudes[i % _currentLS.levelFrequencyAndAmplitude.amplitudes.Length] *
                        Mathf.PerlinNoise(
                            (x + seedOffsetX) * _currentLS.levelFrequencyAndAmplitude.frequecies[i],
                            (y + seedOffsetY) * _currentLS.levelFrequencyAndAmplitude.frequecies[i]);
                }

                // apply the fall off curve to the terrain
                zLevel -= _currentLS.levelFallOff.Evaluate(Mathf.Abs((float)x - (float)_currentLS.levelSize.x / 2f) / (float)_currentLS.levelSize.x / 2f) * _currentLS.fallOffMultiplier;
                zLevel -= _currentLS.levelFallOff.Evaluate(Mathf.Abs((float)y - (float)_currentLS.levelSize.y / 2f) / (float)_currentLS.levelSize.y / 2f) * _currentLS.fallOffMultiplier;

                // apply the terrain height to the grid
                _levelGrid[x, y] = (zLevel / _currentLS.levelFrequencyAndAmplitude.frequecies.Length * 10).ConvertTo<int>();
            }

        // create a dictionary to store the structure positions
        Dictionary<Vector2Int, LevelStructureData> structurePositions = new Dictionary<Vector2Int, LevelStructureData>();

        // place structures
        if (_currentLS.structureDensity > 0 && _currentLS.levelStructures.Length > 0)
        {
            // calculate the rough total structures to be placed
            int totalStructureCount = (int)(_currentLS.levelSize.x * _currentLS.levelSize.y * _currentLS.structureDensity / 10000);
            for (int i = 0; i < totalStructureCount; i++)
            {
                // select a random structure
                LevelStructureData tempStructure = _currentLS.levelStructures[UnityEngine.Random.Range(0, _currentLS.levelStructures.Length)];

                // select a random position for the structure
                Vector2Int tempStructurePos = new Vector2Int(
                    UnityEngine.Random.Range(_currentLS.levelSize.x / 10, _currentLS.levelSize.x - _currentLS.levelSize.x / 10),
                    UnityEngine.Random.Range(_currentLS.levelSize.y / 10, _currentLS.levelSize.y - _currentLS.levelSize.y / 10));

                // check if the structure can be placed (structure does not overlap previously placed structure)
                bool canPlace = true;
                foreach (KeyValuePair<Vector2Int, LevelStructureData> keyValuePair in structurePositions)
                {
                    Vector2 d = ((Vector2)tempStructurePos - (Vector2)keyValuePair.Key).normalized;
                    float dist = Vector2.Distance(tempStructurePos, keyValuePair.Key);

                    float distR = (tempStructure.structureRadius * d + keyValuePair.Value.structureRadius * d).magnitude;
                    if (dist < distR)
                    {
                        canPlace = false;
                        break;
                    }
                }
                if (!canPlace) continue;

                // define the ground level beneath the structure
                int structureGroundLevel = 0;
                for (int x = tempStructurePos.x - tempStructure.structureRadius.x; x < tempStructurePos.x + tempStructure.structureRadius.x; x++)
                    for (int y = tempStructurePos.y - tempStructure.structureRadius.y; y < tempStructurePos.y + tempStructure.structureRadius.y; y++)
                    {
                        structureGroundLevel += _levelGrid[x, y];
                    }
                structureGroundLevel /= (tempStructure.structureRadius.x * 2) * (tempStructure.structureRadius.y * 2) - tempStructure.groundLevelOffset;
                // check if the structure is placed on water
                if (structureGroundLevel <= _currentLS.fallOffHeight) continue;

                // flatten the ground beneath the structure
                for (int x = Mathf.Clamp(tempStructurePos.x - tempStructure.structureRadius.x - tempStructure.structureBorderRadius.x, 0, _currentLS.levelSize.x); x < Mathf.Clamp(tempStructurePos.x + tempStructure.structureRadius.x + tempStructure.structureBorderRadius.x, 0, _currentLS.levelSize.x); x++)
                    for (int y = Mathf.Clamp(tempStructurePos.y - tempStructure.structureRadius.y - tempStructure.structureBorderRadius.y, 0, _currentLS.levelSize.y); y < Mathf.Clamp(tempStructurePos.y + tempStructure.structureRadius.y + tempStructure.structureBorderRadius.y, 0, _currentLS.levelSize.y); y++)
                    {
                        float t = Mathf.Clamp(Vector2.Distance(tempStructurePos, new Vector2(x, y)) -
                            ((tempStructurePos - new Vector2(x, y)).normalized * tempStructure.structureRadius).magnitude -
                            ((tempStructurePos - new Vector2(x, y)).normalized * tempStructure.structureBorderRadius).magnitude / 2f, 0f, 1f);
                        _levelGrid[x, y] = Mathf.Lerp(structureGroundLevel, _levelGrid[x, y], t).ConvertTo<int>();
                    }

                // place the structure
                tempStructure.structure.PlaceStructure(new Vector3Int(tempStructurePos.x, tempStructurePos.y, structureGroundLevel), tilemap);

                // add the structure to the dictionary
                structurePositions.Add(tempStructurePos, tempStructure);
            }
        }

        Vector2Int[] dir = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // place the tiles on the tilemap
        for (int x = 0; x < _currentLS.levelSize.x; x++)
            for (int y = 0; y < _currentLS.levelSize.y; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, _levelGrid[x, y] > _currentLS.fallOffHeight ? _levelGrid[x, y] : _currentLS.fallOffHeight.ConvertTo<int>() - 1), _levelGrid[x, y] >= _currentLS.fallOffHeight ? groundTop : water);

                // skip the ground fill if the terrain is below the fall off height
                if (_levelGrid[x, y] <= _currentLS.fallOffHeight) continue;

                // fill the ground beneath the surface terrain
                foreach (var d in dir)
                {
                    // skip the direction if the tile is out of bounds
                    if (x + d.x < 0 || x + d.x >= _currentLS.levelSize.x || y + d.y < 0 || y + d.y >= _currentLS.levelSize.y) continue;

                    // if the tile is lower than the current tile, fill the space between the tiles
                    if (_levelGrid[x + d.x, y + d.y] < _levelGrid[x, y] - 1)
                    {
                        for (int i = _levelGrid[x, y] - 1; i > _levelGrid[x + d.x, y + d.y]; i -= 2)
                        {
                            tilemap.SetTile(new Vector3Int(x, y, i), groundFill);
                        }
                    }
                }
            }

        return true;
    }

    public bool GenerateLevel(string levelName)
    {
        foreach (var levelTextureSetting in _levelSettings)
        {
            if (levelTextureSetting.levelName == levelName)
            {
                // set the current level settings to the found level settings
                _currentLS = levelTextureSetting;

                return GenerateLevel();
            }
        }

        // if the level name is not found, return false
        Debug.LogError("Level name not found!");
        return false;
    }

    public bool GenerateLevel(int seed)
    {
        // set the seed for the random number generator
        _seed = seed;
        return GenerateLevel();
    }

    public bool GenerateLevel(string levelName, int seed)
    {
        foreach (var levelTextureSetting in _levelSettings)
        {
            if (levelTextureSetting.levelName == levelName)
            {
                // set the current level settings to the found level settings
                _currentLS = levelTextureSetting;
                _seed = seed;

                return GenerateLevel();
            }
        }

        // if the level name is not found, return false
        Debug.LogError("Level name not found!");
        return false;
    }

    public bool GenerateLevel(LevelSettings levelSettings)
    {
        // set the current level settings to the given level settings
        _currentLS = levelSettings;

        return GenerateLevel();
    }

    public bool GenerateLevel(LevelSettings levelSettings, int seed)
    {
        // set the current level settings to the given level settings
        _currentLS = levelSettings;
        _seed = seed;

        return GenerateLevel();
    }

    [Serializable]
    public struct LevelSettings
    {
        [SerializeField] public string levelName;
        [Tooltip("Controls the terrain generation for the level. Values work best when scaled inversly to eachother."), SerializeField, Space] public LevelFrequencyAndAmplitude levelFrequencyAndAmplitude;
        [SerializeField, Space] public Vector2Int levelSize;
        [SerializeField] public AnimationCurve levelFallOff;
        [SerializeField] public float fallOffMultiplier;
        [SerializeField] public float fallOffHeight;
        [SerializeField, Space, Tooltip("Density equal to 1 will yield one structure for every 1000 tiles. (100x100 space)")] public float structureDensity;
        [SerializeField] public LevelStructureData[] levelStructures;

        public LevelSettings(string levelName, LevelFrequencyAndAmplitude levelFrequencyAndAmplitude, Vector2Int levelSize, AnimationCurve levelFallOff, float fallOffMultiplier, float fallOffHeight, LevelStructureData[] levelStructures, float structureDensity)
        {
            this.levelName = levelName;
            this.levelFrequencyAndAmplitude = levelFrequencyAndAmplitude;
            this.levelSize = levelSize;
            this.levelFallOff = levelFallOff;
            this.fallOffMultiplier = fallOffMultiplier;
            this.fallOffHeight = fallOffHeight;
            this.levelStructures = levelStructures;
            this.structureDensity = structureDensity;
        }

        public override bool Equals(object obj)
        {
            return obj is LevelSettings settings &&
                   levelName == settings.levelName &&
                   levelFrequencyAndAmplitude.Equals(settings.levelFrequencyAndAmplitude) &&
                   levelSize.Equals(settings.levelSize) &&
                   levelFallOff == settings.levelFallOff &&
                   fallOffMultiplier == settings.fallOffMultiplier &&
                   fallOffHeight == settings.fallOffHeight;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(levelName, levelFrequencyAndAmplitude, levelSize, levelFallOff, fallOffMultiplier, fallOffHeight);
        }
    }

    [Serializable]
    public struct LevelFrequencyAndAmplitude
    {
        [Tooltip("Controls the grain or roughness of resulting terrain."), SerializeField] public float[] frequecies;
        [Tooltip("Controls the height of resulting terrain."), SerializeField] public float[] amplitudes;

        public LevelFrequencyAndAmplitude(float[] frequecies, float[] amplitudes)
        {
            this.frequecies = frequecies;
            this.amplitudes = amplitudes;
        }

        public override bool Equals(object obj)
        {
            return obj is LevelFrequencyAndAmplitude frequecies &&
                   this.frequecies == frequecies.frequecies &&
                   amplitudes == frequecies.amplitudes;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(frequecies, amplitudes);
        }
    }

    [Serializable]
    public struct LevelStructureData
    {
        [SerializeField] public LevelStructureObject structure;
        [SerializeField] public int groundLevelOffset;
        [SerializeField, Tooltip("Defines the radius of the flattened ground beneath the structure.")] public Vector2Int structureRadius;
        [SerializeField, Tooltip("Defines the thickness of the transitioning boarder terrain.")] public Vector2Int structureBorderRadius;
    }
}
