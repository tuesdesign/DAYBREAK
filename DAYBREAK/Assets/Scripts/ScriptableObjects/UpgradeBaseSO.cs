using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UpgradeB", menuName = "ScriptableObjects/Upgrade", order = 1)]
public class UpgradeBaseSO : ScriptableObject
{
    public string upgradeName; // Name of the upgrade
    public int maxLevel;
    public int level; // Current level of the upgrade
    public Image image;
    public string description;
    

    public List<UpgradeLevels> upgradeLevels = new List<UpgradeLevels>();

    [System.Serializable] // Make it serializable to be visible in the inspector
    public class UpgradeLevels
    {
        public bool replacePrevUpgrade;
        public string description;

        public bool usesBasePlayer;
        public BasePlayerStats basePlayerStats; 

        public bool usesExp;
        public ExpStats expStats;

        public bool usesShooting;
        public PlayerShooting playerShooting;

        public bool usesbulletPropersties;
        public BulletProperties bulletProperties;
    }

    [System.Serializable]
    public class BasePlayerStats
    {
        public int maxHealthModifier = 0;
        public bool healsOnApply = true;

        public float moveSpeedIncrease = 0;
    }

    [System.Serializable]
    public class ExpStats
    {
        public float expPickUPRadMod = 0;
        public float expMultiplier = 0;
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
