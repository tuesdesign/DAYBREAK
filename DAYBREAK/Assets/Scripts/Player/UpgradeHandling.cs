using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeHandling : MonoBehaviour
{
    public List<UpgradeBaseSO> upgradeList;
    
    public List<UpgradeBaseSO> FullupgradeList;

    PlayerBase playerBase;
    PlayerShooting shooting;
    PlayerExpHandler expHandler;
    PlayerUI playerUI;
    BulletApplicationHandling bulletApplication;


    // Start is called before the first frame update
    void Start()
    {
        playerBase = FindAnyObjectByType(typeof(PlayerBase)).GetComponent<PlayerBase>();
        shooting = FindAnyObjectByType(typeof(PlayerShooting)).GetComponent<PlayerShooting>();
        expHandler = FindAnyObjectByType(typeof(PlayerExpHandler)).GetComponent<PlayerExpHandler>();
        playerUI = FindAnyObjectByType<PlayerUI>().GetComponent<PlayerUI>();
        bulletApplication = FindAnyObjectByType<BulletApplicationHandling>().GetComponent<BulletApplicationHandling>();
    }

    public UpgradeBaseSO GetUpgrade()
    {
        return FullupgradeList[Random.Range(0,FullupgradeList.Count)];
    }

    public void IncreaseUpgrade(UpgradeBaseSO upgradeToAdd)
    {
        var existingUpgrade = upgradeList.Find(u => u.upgradeName == upgradeToAdd.upgradeName);

        if (existingUpgrade != null)
        {
            // Increase the level of the existing upgrade in the list
            //existingUpgrade.level++; REINSTATE LATER ALANNIS
            ApplyUpgrade(existingUpgrade);  // Apply changes to the stored instance
        }
        else
        {

            var newUpgrade = Instantiate(upgradeToAdd);
            newUpgrade.level = 0; // Start at level 0 if that's your base level
            upgradeList.Add(newUpgrade);
            ApplyUpgrade(newUpgrade);  // Apply the newly added upgrade
        }
        
    }

    public void ApplyUpgrade(UpgradeBaseSO upgradeToApply)
    {
        //if (upgradeToApply == null || upgradeToApply.level < 0 || upgradeToApply.level >= upgradeToApply.maxLevel)
        //{
        //    return; //used if the upgrade is at its max or any errors in the lvel occur
        //}
        
        Debug.Log("Applying upgrade: " + upgradeToApply.upgradeName + " at level " + upgradeToApply.level);

        PlayerPrefs.SetInt("UpgradesApplied", PlayerPrefs.GetInt("UpgradesApplied") + 1);
        
        var upgradeLevel = upgradeToApply.upgradeLevels[0];
        //var upgradeLevel = upgradeToApply.upgradeLevels[upgradeToApply.level];

        if (upgradeLevel.usesBasePlayer)
        {
            playerBase.UpdateMaxHealth(upgradeLevel.basePlayerStats.maxHealthModifier, upgradeLevel.basePlayerStats.healsOnApply);
            playerBase.speedModifier += upgradeLevel.basePlayerStats.moveSpeedIncrease;
            playerBase.invincibilityTimeModifier += upgradeLevel.basePlayerStats.invincibilityFrameMod;
            playerBase.shield += upgradeLevel.basePlayerStats.sheildMod;
            playerBase.ToggleSheild();
            playerBase.dodgeChange += upgradeLevel.basePlayerStats.dodgeChanceMod;
        }

        if (upgradeLevel.usesExp)
        {
            expHandler.UpdateRadius(upgradeLevel.expStats.expPickUPRadMod);
            expHandler.UpdageEXPMultiplier(upgradeLevel.expStats.expMultiplier);
        }
        if (upgradeLevel.usesShooting)
        {
            shooting.maxAmmoMod += upgradeLevel.playerShooting.maxAmmoModifier;
            shooting.shootdelayMod += upgradeLevel.playerShooting.shootDelayModifier;
            shooting.bspeedMod += upgradeLevel.playerShooting.bulletSpeedModifier;
            shooting.BulletsPerShot += upgradeLevel.playerShooting.bulletsPerShotModifier;
            shooting.BulletSpread += upgradeLevel.playerShooting.bulletSpreadModifier;

            if (upgradeToApply.upgradeName == "Max Ammo Up")
                playerUI.UpdateAmmoDisplay();
                
        }
        if (upgradeLevel.usesbulletPropersties)
        {
            //i cry
            bulletApplication.bulletDamageMod += upgradeLevel.bulletProperties.damageMod;

            if (upgradeLevel.bulletProperties.canBurn)
            {
                bulletApplication.canBurn = upgradeLevel.bulletProperties.canBurn;
                bulletApplication.burnChance += upgradeLevel.bulletProperties.burnChance;
                bulletApplication.shotsBetweenBurn = upgradeLevel.bulletProperties.shotsBetweenBurn;
            }

            if (upgradeLevel.bulletProperties.canPoison)
            {
                bulletApplication.canPoison = upgradeLevel.bulletProperties.canPoison;
                bulletApplication.poisonChance += upgradeLevel.bulletProperties.poisonChance;
                bulletApplication.shotsBetweenPoison = upgradeLevel.bulletProperties.shotsBetweenPoison;
            }

            if (upgradeLevel.bulletProperties.canExplode)
            {
                bulletApplication.canExplode = upgradeLevel.bulletProperties.canExplode;
                bulletApplication.explosionRange += upgradeLevel.bulletProperties.explosionRange;
                bulletApplication.explosionDamage = upgradeLevel.bulletProperties.explosionDamage;
            }

            if (upgradeLevel.bulletProperties.canFreeze)
            {
                bulletApplication.canFreeze = upgradeLevel.bulletProperties.canFreeze;
                bulletApplication.freezeChance += upgradeLevel.bulletProperties.freezeChance;
                bulletApplication.shotsBetweenFreeze = upgradeLevel.bulletProperties.shotsBwteenFreeze;
            } 

            if (upgradeLevel.bulletProperties.canSlow)
            {
                bulletApplication.canSlow = upgradeLevel.bulletProperties.canSlow;
                bulletApplication.slowChance += upgradeLevel.bulletProperties.slowChance;
                bulletApplication.shotsBetweenSlow = upgradeLevel.bulletProperties.shotsBwteenSlow;
            }

            if (upgradeLevel.bulletProperties.pierceAmount > 0)
            {
                bulletApplication.pierceAmount += upgradeLevel.bulletProperties.pierceAmount;
            }
            
        }
    }
}