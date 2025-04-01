using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Unity.VisualScripting;

public class ShareOnSocialMedia : MonoBehaviour
{
    [SerializeField] TMP_Text txtDate;

    public void ShareScore()
    {
        DateTime dt = DateTime.Now; // Get current date
        txtDate.text = $"{dt.Month}/{dt.Day}/{dt.Year}";

        StartCoroutine(TakeScreenshotShare());
    }

    IEnumerator TakeScreenshotShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D tx = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tx.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tx.Apply();

        string path = Path.Combine(Application.temporaryCachePath, "sharedImage.png");
        File.WriteAllBytes(path, tx.EncodeToPNG());

        Destroy(tx);
        
        new NativeShare().AddFile(path).SetTitle("My Daily Daybreak Score").SetText("This is how long I survived today in Daybreak! Can you match it?").Share();

        txtDate.text = "";
    }
}