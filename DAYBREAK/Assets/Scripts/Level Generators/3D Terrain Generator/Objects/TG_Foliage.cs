using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Foliage", menuName = "Terrain Generator/Foliage")]
public class TG_Foliage : ScriptableObject
{
    public GameObject[] prefabs;
    public FoliageRotation rotType = FoliageRotation.None;
    public Vector3 rotationLimit;
    public float minScale = 1;
    public float maxScale = 1;
    public float heightOffset = 0;

    [Tooltip("foliageDensity of 1 will yield 1 instance of foliage per 10 x 10 map size")] public float density = 1;

    public Material[] materials;


    public void InstanceFolliage(Vector3 pos, Transform parent)
    {
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

        Quaternion rotation = prefab.transform.rotation;

        switch (rotType)
        {
            // Random rotation
            case FoliageRotation.RandomFull:
                rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)) * prefab.transform.rotation;
                break;

            // Random Y rotation
            case FoliageRotation.RandomY:
                rotation = Quaternion.Euler(0, Random.Range(0, 360), 0) * prefab.transform.rotation;
                break;

            // Limited rotation
            case FoliageRotation.Limited:
                rotation = Quaternion.Euler(
                    Random.Range(-rotationLimit.x / 2, rotationLimit.x / 2), 
                    Random.Range(-rotationLimit.y / 2, rotationLimit.y / 2), 
                    Random.Range(-rotationLimit.z / 2, rotationLimit.z / 2)) * prefab.transform.rotation;
                break;
        }

        GameObject instance = Instantiate(prefab, pos, rotation, parent);
        instance.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
        instance.transform.localPosition += (Vector3.up * instance.transform.localScale.y / 2) - Vector3.up * heightOffset;
        instance.GetComponent<Renderer>().material = materials[Random.Range(0, materials.Length)];
    }

    public enum FoliageRotation
    {
        RandomFull,
        RandomY,
        Limited,
        None
    }
}
