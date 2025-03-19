using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character", order = 1)]
public class PlayerSO : ScriptableObject
{
    [SerializeField]
    public int maxHealth = 10;
    public float speed = 2.5f;

    [Header("Shooting")]
    public int maxammo = 6;
    public float bulletspeed = 10f;
    public GameObject bulletType;
    public float reloadTime = 0.75f;
    public float shootDelay = 0.5f;

    [Header("Visuals and Animation")]
    public RuntimeAnimatorController controller;
    public UpgradeBaseSO baseUpgradables;
}
