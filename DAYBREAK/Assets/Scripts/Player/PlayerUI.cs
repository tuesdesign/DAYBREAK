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
    [SerializeField] Slider healthBar;
    [Tooltip("This is the exp bar.")]
    [SerializeField] Slider expBar;
    [Tooltip("The text bar to describe how much ammo the player has in comparisson to their maximum ammo")]
    [SerializeField] TMP_Text ammoTextBar;


    private void Start()
    {
        player = GetComponent<PlayerBase>();
        playerExpHandler = GetComponent<PlayerExpHandler>();
        playerShooting = GetComponent<PlayerShooting>();

        if (healthBar != null)
        {
            healthBar.maxValue = player.MaxHealth + player.maxHealthModifier;
            healthBar.value = player.MaxHealth + player.maxHealthModifier;
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
        if (healthBar != null)
        {
            healthBar.value = player.CurHealth;
        }
    }

    public void UpdateAmmoCount()
    {
        if (ammoTextBar != null)
        {
            ammoTextBar.text = playerShooting.AmmoCount + " / " + playerShooting.MaxAmmo;
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
