using System;
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

                tilemap.SetTile(new Vector3Int(x, y, _levelGrid[x, y] > _currentLS.fallOffHeight ? _levelGrid[x, y] : _currentLS.fallOffHeight.ConvertTo<int>() - 1), _levelGrid[x, y] >= _currentLS.fallOffHeight ? groundTop : water);
            }

        Vector2Int[] dir = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        for (int x = 0; x < _currentLS.levelSize.x; x++)
            for (int y = 0; y < _currentLS.levelSize.y; y++)
            {
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

        public LevelSettings(string levelName, LevelFrequencyAndAmplitude levelFrequencyAndAmplitude, Vector2Int levelSize, float fallOffMultiplier, AnimationCurve levelFallOff, float fallOffHeight)
        {
            this.levelName = levelName;
            this.levelFrequencyAndAmplitude = levelFrequencyAndAmplitude;
            this.levelSize = levelSize;
            this.levelFallOff = levelFallOff;
            this.fallOffMultiplier = fallOffMultiplier;
            this.fallOffHeight = fallOffHeight;
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
}
