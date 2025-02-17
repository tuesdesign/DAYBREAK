using System.Collections;
using System.Collections.Generic;
using UI.Scripts;
using UI.Scripts.Upgrades;
using UnityEngine;

public class PlayerExpHandler : MonoBehaviour
{
    PlayerUI playerUI;
    [SerializeField] float expPickUpRadius = 5f;

    int exp = 0;
    [SerializeField] int level = 1;
    [Tooltip("This determines how much it takes to level up initially")]
    [SerializeField] int levelIncrement = 100;
    [Tooltip("NOT IMPLEMENTED \n Rate of increase of exp needed for each level")]
    [SerializeField] AnimationCurve incrementRate; //does nothing for now

    //Modifiers for upgrades 
    [HideInInspector] public float expPickUPRadMod = 0;
    [HideInInspector] public float expMultiplier = 1;

    [SerializeField] private UpgradeManagerMenu _upgradeManagerMenu;

    public int Exp { get => exp; set => exp = value; }
    public int Level { get => level; set => level = value; }
    public int LevelIncrement { get => levelIncrement; set => levelIncrement = value; }

    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        GetComponent<SphereCollider>().radius = expPickUpRadius;
    }

    public void GainEXP(int amount)
    {
        exp += Mathf.RoundToInt(amount * expMultiplier);
        
        PlayerPrefs.SetInt("XpEarned", PlayerPrefs.GetInt("XpEarned") + Mathf.RoundToInt(amount * expMultiplier));
        
        playerUI.UpdateExpBar();
        
        if (exp >= levelIncrement)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        exp -= levelIncrement;
        level++;
        levelIncrement += 10 + ((int)(level/5)*2);

        // INSERT A CALL TO SPAWN THE UPGRADE MENU AND PAUSE THE TIME  (ALSO ENSURE THAT AFTER SELECTING THE UPGRADE MENU THAT TIME REVERTS)
        _upgradeManagerMenu.PopulateMenu();

        playerUI.UpdateExpBar();
        
        if (exp >= levelIncrement)
        {
            LevelUp();
        }
        
        playerUI.UpdateExpBar();
    }

    public void UpdateRadius(float amount)
    {
        expPickUpRadius += amount;
        GetComponent<SphereCollider>().radius = expPickUpRadius + expPickUPRadMod;
    }
    
    public void UpdageEXPMultiplier(float amount)
    {
       expMultiplier += amount;
    }
}
