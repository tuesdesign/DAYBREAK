using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class LevelTextureManager : MonoBehaviour
{
    [SerializeField] public int seed;
    [Space, SerializeField] LevelTextureSettings[] _levelTextureSettings;
    [Space, SerializeField] public string filePath;

    [SerializeField] UnityEngine.UI.Image debugImage;

    private void Awake()
    {
        Texture2D texture = GenerateLevelTexture(_levelTextureSettings[0]);
        debugImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        debugImage.SetNativeSize();
    }

    private Texture2D GenerateLevelTexture(LevelTextureSettings ls)
    {
        Texture2D texture = new Texture2D(ls.levelSize.x, ls.levelSize.y);

        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < ls.levelSize.x; x++)
        {
            for (int y = 0; y < ls.levelSize.y; y++)
            {
                texture.SetPixel(x, y, random.Next(0, 2) == 0 ? Color.black : Color.white);
            }
        }

        Vector2 tempScale = new Vector2(texture.width / (ls.smoothness + 1), texture.height / (ls.smoothness + 1));
        Texture2D tempTex = new Texture2D((int)tempScale.x, (int)tempScale.y);

        texture.Apply();

        Graphics.ConvertTexture(texture, tempTex);
        Graphics.ConvertTexture(tempTex, texture);

        string path = filePath + $"/levelTexture_{ls.levelName}_{seed}_{ls.smoothness}_{ls.levelSize}.jpg";
        System.IO.File.WriteAllBytes(path, texture.EncodeToJPG());
        AssetDatabase.Refresh();

        return texture;
    }

    [Serializable]
    public struct LevelTextureSettings
    {
        public string levelName;
        [Range(0, 100)] public float smoothness;
        public Vector2Int levelSize;
    }
}
