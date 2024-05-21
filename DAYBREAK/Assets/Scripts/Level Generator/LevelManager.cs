using System;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public int _seed;
    [SerializeField] public Vector2Int _levelSize;
    private float[,] _levelGrid;

    [SerializeField] public Tilemap tilemap;
    [SerializeField] public Tile grass, water;

    [Space, SerializeField] LevelSettings[] _levelTextureSettings;
    private LevelSettings _currentLevelTextureSettings;

    private void Awake()
    {
        GenerateLevel();
    }

    public bool GenerateLevel()
    {
        if (_currentLevelTextureSettings.Equals(new LevelSettings())) _currentLevelTextureSettings = _levelTextureSettings[0];

        UnityEngine.Random.InitState(_seed);
        float seedOffsetX = UnityEngine.Random.Range(-100000f, 100000f);
        float seedOffsetY = UnityEngine.Random.Range(-100000f, 100000f);

        _levelGrid = new float[_currentLevelTextureSettings.levelSize.x, _currentLevelTextureSettings.levelSize.y];

        for (int x = 0; x < _currentLevelTextureSettings.levelSize.x; x++)
        {
            for (int y = 0; y < _currentLevelTextureSettings.levelSize.y; y++)
            {
                float zLevel = 0;

                for (int i = 0; i < _currentLevelTextureSettings.frequecies.Length; i++)
                {
                    zLevel += _currentLevelTextureSettings.amplitudes[i % _currentLevelTextureSettings.amplitudes.Length] * 
                        Mathf.PerlinNoise(
                            (x + seedOffsetX) * _currentLevelTextureSettings.frequecies[i], 
                            (y + seedOffsetY) * _currentLevelTextureSettings.frequecies[i]);
                }

                zLevel -= _currentLevelTextureSettings.levelFallOff.Evaluate(Mathf.Abs((float)x - (float)_currentLevelTextureSettings.levelSize.x / 2f) / (float)_currentLevelTextureSettings.levelSize.x / 2f) * _currentLevelTextureSettings.fallOffMultiplier;
                zLevel -= _currentLevelTextureSettings.levelFallOff.Evaluate(Mathf.Abs((float)y - (float)_currentLevelTextureSettings.levelSize.y / 2f) / (float)_currentLevelTextureSettings.levelSize.y / 2f) * _currentLevelTextureSettings.fallOffMultiplier;

                _levelGrid[x, y] = zLevel / _currentLevelTextureSettings.frequecies.Length;

                tilemap.SetTile(new Vector3Int(x, y, _levelGrid[x, y] > _currentLevelTextureSettings.voidLevel ? (_levelGrid[x, y] * 10).ConvertTo<int>() : (_currentLevelTextureSettings.voidLevel * 10).ConvertTo<int>() - 1), _levelGrid[x, y] > _currentLevelTextureSettings.voidLevel ? grass : water);
            }
        }

        return true;
    }

    public bool GenerateLevel(string levelName)
    {
        foreach (var levelTextureSetting in _levelTextureSettings)
        {
            if (levelTextureSetting.levelName == levelName)
            {
                _currentLevelTextureSettings = levelTextureSetting;

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
        foreach (var levelTextureSetting in _levelTextureSettings)
        {
            if (levelTextureSetting.levelName == levelName)
            {
                _currentLevelTextureSettings = levelTextureSetting;
                _seed = seed;

                return GenerateLevel();
            }
        }

        Debug.LogError("Level name not found!");
        return false;
    }

    public bool GenerateLevel(LevelSettings levelSettings)
    {
        _currentLevelTextureSettings = levelSettings;

        return GenerateLevel();
    }

    public bool GenerateLevel(LevelSettings levelSettings, int seed)
    {
        _currentLevelTextureSettings = levelSettings;
        _seed = seed;

        return GenerateLevel();
    }

    [Serializable] public struct LevelSettings
    {
        public string levelName;
        [Range(0, 10)] public int smoothness;
        public float[] frequecies, amplitudes;
        public Vector2Int levelSize;
        public AnimationCurve levelFallOff;
        public float fallOffMultiplier;
        public float voidLevel;

        public LevelSettings(string levelName, int smoothness, float[] frequecies, float[] amplitudes, Vector2Int levelSize, float fallOffMultiplier, AnimationCurve levelFallOff, float voidLevel)
        {
            this.levelName = levelName;
            this.smoothness = smoothness;
            this.frequecies = frequecies;
            this.amplitudes = amplitudes;
            this.levelSize = levelSize;
            this.levelFallOff = levelFallOff;
            this.fallOffMultiplier = fallOffMultiplier;
            this.voidLevel = voidLevel;
        }

        public override bool Equals(object obj)
        {
            return obj is LevelSettings settings &&
                   levelName == settings.levelName &&
                   smoothness == settings.smoothness &&
                   frequecies == settings.frequecies &&
                   amplitudes == settings.amplitudes &&
                   levelSize.Equals(settings.levelSize) &&
                   levelFallOff == settings.levelFallOff &&
                   fallOffMultiplier == settings.fallOffMultiplier &&
                   voidLevel == settings.voidLevel;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(levelName, smoothness, frequecies, amplitudes, levelSize, levelFallOff, fallOffMultiplier, voidLevel);
        }
    }
}
