using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public Inventory inventory = new Inventory();

    public void SaveToJson()
    {
        string noteData = JsonUtility.ToJson(inventory);
        string filePath = Application.persistentDataPath + "/NotesData.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, noteData);
        Debug.Log("Save Successful");
    }

    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/NotesData.json";
        string noteData = System.IO.File.ReadAllText(filePath);

        inventory = JsonUtility.FromJson<Inventory>(noteData);
        Debug.Log("Load Successful");
    }
}

[Serializable]
public class Inventory
{
    public List<Notes> notes = new List<Notes>();
}

[Serializable]
public class Notes
{
    public int noteNum;
    public bool isUnlocked;
}