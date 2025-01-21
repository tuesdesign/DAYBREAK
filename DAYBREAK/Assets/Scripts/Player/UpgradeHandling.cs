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


    // Start is called before the first frame update
    void Start()
    {
        playerBase = FindAnyObjectByType(typeof(PlayerBase)).GetComponent<PlayerBase>();
        shooting = FindAnyObjectByType(typeof(PlayerShooting)).GetComponent<PlayerShooting>();
        expHandler = FindAnyObjectByType(typeof(PlayerExpHandler)).GetComponent<PlayerExpHandler>();
        playerUI = FindAnyObjectByType<PlayerUI>().GetComponent<PlayerUI>();
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
        if (upgradeToApply == null || upgradeToApply.level < 0 || upgradeToApply.level >= upgradeToApply.maxLevel)
        {
            return; 
        }
        Debug.Log("Applying upgrade: " + upgradeToApply.upgradeName + " at level " + upgradeToApply.level);

        var upgradeLevel = upgradeToApply.upgradeLevels[upgradeToApply.level];

        if (upgradeLevel.usesBasePlayer)
        {
            playerBase.UpdateMaxHealth(upgradeLevel.basePlayerStats.maxHealthModifier, upgradeLevel.basePlayerStats.healsOnApply);
            playerBase.speedModifier += upgradeLevel.basePlayerStats.moveSpeedIncrease;
            playerBase.invincibilityTimeModifier += upgradeLevel.basePlayerStats.invincibilityFrameMod;
            playerBase.shield += upgradeLevel.basePlayerStats.sheildMod;
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
            {
                playerUI.UpdateAmmoDisplay();
                playerUI.UpdateAmmoCount();
            }
                
        }
        if (upgradeLevel.usesbulletPropersties)
        {
            
        }
    }
}