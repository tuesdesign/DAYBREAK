using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeB", menuName = "ScriptableObjects/Upgrade", order = 1)]
public class UpgradeBaseSO : ScriptableObject
{
    public string upgradeName; // Name of the upgrade
    public int maxLevel;
    public int level; // Current level of the upgrade
    public string description;
    

    public List<UpgradeLevels> upgradeLevels = new List<UpgradeLevels>();

    [System.Serializable] // Make it serializable to be visible in the inspector
    public class UpgradeLevels
    {
        public bool replacePrevUpgrade;
        public string description;

        public BasePlayerStats basePlayerStats;  
        public ExpStats expStats;
        public PlayerShooting playerShooting;
        public BulletProperties bulletProperties;
    }

    [System.Serializable]
    public class BasePlayerStats
    {
        public int maxHealthModifier;
        public bool healsOnApply;

        public float moveSpeedIncrease;
    }

    [System.Serializable]
    public class ExpStats
    {
        public float expPickUPRadMod;
        public float expMultiplier;
    }

    [System.Serializable]
    public class PlayerShooting
    {
        public float bulletSpeedModifier;
        public float shootDelayModifier;
        public int maxAmmoModifier;
        public float reloadTimeModifier;
        
    }

    [System.Serializable]
    public class BulletProperties
    {
        public float speed;
        public float damage;

        public bool canBurn;
        public float burnChance;

        public bool isExplosive;
        public float explosionRange;
        public float explosionDamage;

        public bool canFreeze;
        public float freezeChance;

        public bool canBounce;
        public float bounceAmount;

        public float canPierce;
        public float pierceAmount;
    }
}
