using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureData", menuName = "3D Terrain Generator/Structure Data Object", order = 1)]
public class TG_StructureDataObject : ScriptableObject
{
    public GameObject obj;
    public float offestY;
    public int radius;
}
