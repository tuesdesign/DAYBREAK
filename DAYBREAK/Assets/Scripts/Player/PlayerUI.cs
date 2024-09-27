using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    PlayerBase player;

    [Tooltip("This is the health bar.")]
    [SerializeField] Slider healthBar;
    [Tooltip("This is the exp bar.")]
    [SerializeField] Slider expBar;
    [Tooltip("The text bar to describe how much ammo the player has in comparisson to their maximum ammo")]
    [SerializeField] TMP_Text ammoTextBar;


    private void Start()
    {
        player = GetComponent<PlayerBase>();
        if (healthBar != null)
        {
            healthBar.maxValue = player.MaxHealth;
            healthBar.value = player.CurHealth;
        }

        if (ammoTextBar != null)
        {
            ammoTextBar.text = player.AmmoCount + " / " + player.MaxAmmo;
        }

        if (expBar != null)
        {
            expBar.maxValue = player.LevelIncrement;
            expBar.value = player.Exp;
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
            ammoTextBar.text = player.AmmoCount + " / " + player.MaxAmmo;
        }
    }

    public void UpdateEXPBar()
    {
        if (expBar != null)
        {
            expBar.value = player.Exp;
        }
    }
}
