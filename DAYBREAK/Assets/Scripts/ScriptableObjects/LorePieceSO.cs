using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Lore Piece", menuName = "ScriptableObjects/Lore", order = 1)]
public class LorePieceSO : ScriptableObject
{
    public string lorePieceName;
    public int loreNum;
    [TextArea(10, 100)]
    public string loreInformation;
    public Sprite image;
    public bool unlocked = false;
}
