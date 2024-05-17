using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

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
        Texture2D texture = new Texture2D(ls.levelSize.x, ls.levelSize.y, TextureFormat.RGB24, false);

        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < ls.levelSize.x; x++)
        {
            for (int y = 0; y < ls.levelSize.y; y++)
            {
                texture.SetPixel(x, y, random.Next(0, 2) == 0 ? Color.black : Color.white);
            }
        }

        texture.Apply();

        Vector2 textureSize = new Vector2(texture.width, texture.height);

        TextureScale.Bilinear(texture, textureSize.x / 4, textureSize.y / 4);
        TextureScale.Bilinear(texture, textureSize.x, textureSize.y);

        string path = filePath + $"/levelTexture_{ls.levelName}_{seed}_{ls.levelSize}.jpg";
        System.IO.File.WriteAllBytes(path, texture.EncodeToJPG());

        return texture;
    }

    [Serializable]
    public struct LevelTextureSettings
    {
        public string levelName;
        [Range(0, 10)] public float smoothness;
        public Vector2Int levelSize;
    }
}
