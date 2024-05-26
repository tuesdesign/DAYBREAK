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
            if (_levelSettings.Length <= levelIndex)
            {
                Debug.LogError("Level index out of range! " +
                    ((_levelSettings.Length - 1 == 0) ?
                    "\nCurrently, the only acceptable index is 0!" :
                    $"\nCurrently, the acceptable range of valuse is between 0 and {_levelSettings.Length - 1}!"));
                return;
            }

            string indexedLevelName = _levelSettings[levelIndex].levelName;
            GenerateLevel(indexedLevelName);
        }
        else if (_currentLevelName.Length > 0)
        {
            GenerateLevel(_currentLevelName);
        }
        else
        {
            GenerateLevel();
        }
    }

    public bool GenerateLevel()
    {
        if (_currentLS.Equals(new LevelSettings())) _currentLS = _levelSettings[0];

        UnityEngine.Random.InitState(_seed);
        float seedOffsetX = UnityEngine.Random.Range(-100000f, 100000f);
        float seedOffsetY = UnityEngine.Random.Range(-100000f, 100000f);

        _levelGrid = new int[_currentLS.levelSize.x, _currentLS.levelSize.y];

        // Generate the terrain
        for (int x = 0; x < _currentLS.levelSize.x; x++)
            for (int y = 0; y < _currentLS.levelSize.y; y++)
            {
                float zLevel = 0;

                for (int i = 0; i < _currentLS.levelFrequencyAndAmplitude.frequecies.Length; i++)
                {
                    zLevel += _currentLS.levelFrequencyAndAmplitude.amplitudes[i % _currentLS.levelFrequencyAndAmplitude.amplitudes.Length] *
                        Mathf.PerlinNoise(
                            (x + seedOffsetX) * _currentLS.levelFrequencyAndAmplitude.frequecies[i],
                            (y + seedOffsetY) * _currentLS.levelFrequencyAndAmplitude.frequecies[i]);
                }

                zLevel -= _currentLS.levelFallOff.Evaluate(Mathf.Abs((float)x - (float)_currentLS.levelSize.x / 2f) / (float)_currentLS.levelSize.x / 2f) * _currentLS.fallOffMultiplier;
                zLevel -= _currentLS.levelFallOff.Evaluate(Mathf.Abs((float)y - (float)_currentLS.levelSize.y / 2f) / (float)_currentLS.levelSize.y / 2f) * _currentLS.fallOffMultiplier;

                _levelGrid[x, y] = (zLevel / _currentLS.levelFrequencyAndAmplitude.frequecies.Length * 10).ConvertTo<int>();
            }

        Dictionary<Vector2Int, LevelStructure> structurePositions = new Dictionary<Vector2Int, LevelStructure>();

        // place structures
        if (_currentLS.structureDensity > 0 && _currentLS.levelStructures.Length > 0)
        {
            // calculate the rough total structures to be placed
            int totalStructureCount = (int)(_currentLS.levelSize.x * _currentLS.levelSize.y * _currentLS.structureDensity / 10000);
            for (int i = 0; i < totalStructureCount; i++)
            {
                // select a random structure
                LevelStructure tempStructure = _currentLS.levelStructures[UnityEngine.Random.Range(0, _currentLS.levelStructures.Length)];

                // select a random position for the structure
                Vector2Int tempStructurePos = new Vector2Int(
                    UnityEngine.Random.Range(_currentLS.levelSize.x / 10, _currentLS.levelSize.x - _currentLS.levelSize.x / 10),
                    UnityEngine.Random.Range(_currentLS.levelSize.y / 10, _currentLS.levelSize.y - _currentLS.levelSize.y / 10));

                // check if the structure can be placed (structure does not overlap previously placed structure)
                bool canPlace = true;
                foreach (KeyValuePair<Vector2Int, LevelStructure> keyValuePair in structurePositions)
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

                // flatten ground for structure
                int structureGroundLevel = 0;
                for (int x = tempStructurePos.x - tempStructure.structureRadius.x; x < tempStructurePos.x + tempStructure.structureRadius.x; x++)
                    for (int y = tempStructurePos.y - tempStructure.structureRadius.y; y < tempStructurePos.y + tempStructure.structureRadius.y; y++)
                    {
                        structureGroundLevel += _levelGrid[x, y];
                    }
                structureGroundLevel /= (tempStructure.structureRadius.x * 2) * (tempStructure.structureRadius.y * 2);
                if (structureGroundLevel <= _currentLS.fallOffHeight) continue;

                for (int x = Mathf.Clamp(tempStructurePos.x - tempStructure.structureRadius.x - tempStructure.structureBorderRadius.x, 0, _currentLS.levelSize.x); x < Mathf.Clamp(tempStructurePos.x + tempStructure.structureRadius.x + tempStructure.structureBorderRadius.x, 0, _currentLS.levelSize.x); x++)
                    for (int y = Mathf.Clamp(tempStructurePos.y - tempStructure.structureRadius.y - tempStructure.structureBorderRadius.y, 0, _currentLS.levelSize.y); y < Mathf.Clamp(tempStructurePos.y + tempStructure.structureRadius.y + tempStructure.structureBorderRadius.y, 0, _currentLS.levelSize.y); y++)
                    {
                        float t = Mathf.Clamp(Vector2.Distance(tempStructurePos, new Vector2(x, y)) -
                            ((tempStructurePos - new Vector2(x, y)).normalized * tempStructure.structureRadius).magnitude -
                            ((tempStructurePos - new Vector2(x, y)).normalized * tempStructure.structureBorderRadius).magnitude / 2f, 0f, 1f);
                        _levelGrid[x, y] = Mathf.Lerp(structureGroundLevel, _levelGrid[x, y], t).ConvertTo<int>();
                    }

                structurePositions.Add(tempStructurePos, tempStructure);

                // place debug tile
                tilemap.SetTile(new Vector3Int(tempStructurePos.x, tempStructurePos.y, _levelGrid[tempStructurePos.x, tempStructurePos.y] + 10), debugTile);
            }
        }

        // Place ground Tiles
        Vector2Int[] dir = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        for (int x = 0; x < _currentLS.levelSize.x; x++)
            for (int y = 0; y < _currentLS.levelSize.y; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, _levelGrid[x, y] > _currentLS.fallOffHeight ? _levelGrid[x, y] : _currentLS.fallOffHeight.ConvertTo<int>() - 1), _levelGrid[x, y] >= _currentLS.fallOffHeight ? groundTop : water);

                if (_levelGrid[x, y] <= _currentLS.fallOffHeight) continue;

                foreach (var d in dir)
                {
                    if (x + d.x < 0 || x + d.x >= _currentLS.levelSize.x || y + d.y < 0 || y + d.y >= _currentLS.levelSize.y) continue;

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
                _currentLS = levelTextureSetting;

                return GenerateLevel();
            }
        }

        Debug.LogError("Level name not found!");
        return false;
    }

    public bool GenerateLevel(int seed)
    {
        _seed = seed;
        return GenerateLevel();
    }

    public bool GenerateLevel(string levelName, int seed)
    {
        foreach (var levelTextureSetting in _levelSettings)
        {
            if (levelTextureSetting.levelName == levelName)
            {
                _currentLS = levelTextureSetting;
                _seed = seed;

                return GenerateLevel();
            }
        }

        Debug.LogError("Level name not found!");
        return false;
    }

    public bool GenerateLevel(LevelSettings levelSettings)
    {
        _currentLS = levelSettings;

        return GenerateLevel();
    }

    public bool GenerateLevel(LevelSettings levelSettings, int seed)
    {
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
        [SerializeField] public LevelStructure[] levelStructures;

        public LevelSettings(string levelName, LevelFrequencyAndAmplitude levelFrequencyAndAmplitude, Vector2Int levelSize, AnimationCurve levelFallOff, float fallOffMultiplier, float fallOffHeight, LevelStructure[] levelStructures, float structureDensity)
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
    public struct LevelStructure
    {
        [SerializeField] public string structureName;
        [SerializeField] public Vector2Int structureSize;
        [SerializeField, Tooltip("Defines the radius of the flattened ground beneath the structure.")] public Vector2Int structureRadius;
        [SerializeField, Tooltip("Defines the thickness of the transitioning boarder terrain.")] public Vector2Int structureBorderRadius;
    }
}
