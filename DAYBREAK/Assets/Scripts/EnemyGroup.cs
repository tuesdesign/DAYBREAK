using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feel;
using Unity.VisualScripting;
using UnityEngine;
using Utility.Simple_Scripts;

public class EnemyGroup : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    private void OnEnable()
    {
        foreach (var enemy in enemies)
            SsObjectPool.GetObject(enemy.name, enemy, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }
}
