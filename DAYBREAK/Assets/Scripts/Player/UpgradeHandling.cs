using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHandling : MonoBehaviour
{
    public List<UpgradeBaseSO> upgradeList;

    PlayerBase playerBase;
    PlayerShooting shooting;
    PlayerExpHandler expHandler;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyUpgrade(UpgradeBaseSO upgradeToAdd)
    {
        UpgradeBaseSO upgrade = upgradeList.Find(u => u.upgradeName == upgradeToAdd.upgradeName);

        if (upgrade != null&& upgrade.level >= upgrade.maxLevel)
        {
            upgrade.level++;


        }
    }
    public void UpdateUpgrades()
    {

    }


}
