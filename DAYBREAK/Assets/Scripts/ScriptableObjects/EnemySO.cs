using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/Enemy", order = 1)]
public class EnemySO : ScriptableObject
{
    [Tooltip("Enemy movement speed")]
    [SerializeField] public float speed = 5f;
    [Tooltip("Health")]
    [SerializeField] public float maxHealth = 3f;
    [Tooltip("damage")]
    [SerializeField] public int damage = 2;

    [Tooltip("The type of exp that the enemy drops when kiled")]
    [SerializeField] public GameObject expDrop;
    [SerializeField] public GameObject altExpDrop;
    [SerializeField][Range(1f, 100f)] public float expDropChance = 100;
}
