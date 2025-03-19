using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChanger : MonoBehaviour
{
    PlayerBase playerBase;
    UpgradeHandling upgradeHandling;
    public void ChangeCharacter(PlayerSO character)
    {
        playerBase.ApplyCharacterStats(character);
        upgradeHandling.ApplyUpgrade(character.baseUpgradables);   
    }
}
