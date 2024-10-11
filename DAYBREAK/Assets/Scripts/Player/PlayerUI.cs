using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    PlayerBase player;
    PlayerExpHandler playerExpHandler;
    PlayerShooting playerShooting;

    [Tooltip("This is the health bar.")]
    [SerializeField] Slider healthBar1;
    [SerializeField] Slider healthBar2;
    [Tooltip("This is the exp bar.")]
    [SerializeField] Slider expBar;
    [Tooltip("The text bar to describe how much ammo the player has in comparisson to their maximum ammo")]
    [SerializeField] TMP_Text ammoTextBar;


    private void Start()
    {
        player = GetComponent<PlayerBase>();
        playerExpHandler = GetComponent<PlayerExpHandler>();
        playerShooting = GetComponent<PlayerShooting>();

        if (healthBar1 != null && healthBar2 != null)
        {
            healthBar1.maxValue = player.MaxHealth + player.maxHealthModifier;
            healthBar1.value = player.MaxHealth + player.maxHealthModifier;
            
            healthBar2.maxValue = player.MaxHealth + player.maxHealthModifier;
            healthBar2.value = player.MaxHealth + player.maxHealthModifier;
        }

        if (ammoTextBar != null)
        {
            ammoTextBar.text = playerShooting.AmmoCount + " / " + (playerShooting.MaxAmmo + playerShooting.maxAmmoMod);
        }

        if (expBar != null)
        {
            expBar.maxValue = playerExpHandler.LevelIncrement;
            expBar.value = playerExpHandler.Exp;
        }
    }
    public void UpdateHealthBar()
    {
        if (healthBar1 != null)
        {
            healthBar1.value = player.CurHealth;
            healthBar1.maxValue = player.MaxHealth + player.maxHealthModifier;
        }
        if (healthBar2 != null)
        {
            healthBar2.value = player.CurHealth;
            healthBar2.maxValue = player.MaxHealth + player.maxHealthModifier;
        }
    }

    public void UpdateAmmoCount()
    {
        if (ammoTextBar != null)
        {
            ammoTextBar.text = playerShooting.AmmoCount + " / " + (playerShooting.MaxAmmo+ playerShooting.maxAmmoMod);
        }
    }

    public void UpdateEXPBar()
    {
        if (expBar != null)
        {
            expBar.value = playerExpHandler.Exp;
        }
    }
}
