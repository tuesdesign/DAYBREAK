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
    [Tooltip("This is the exp bar.")]
    [SerializeField] Slider expBar;
    [Tooltip("The text bar to describe how much ammo the player has in comparison to their max ammo")]
    [SerializeField] TMP_Text ammoTextBar;


    private void Start()
    {
        player = GetComponent<PlayerBase>();
        playerExpHandler = GetComponent<PlayerExpHandler>();
        playerShooting = GetComponent<PlayerShooting>();

        if (healthBar1 == null || ammoTextBar == null || expBar == null)
            Debug.LogError("Missing variable assignment!");
        
        // Assign starting values
        healthBar1.maxValue = player.MaxHealth + player.maxHealthModifier;
        healthBar1.value = player.MaxHealth + player.maxHealthModifier;

        ammoTextBar.text = playerShooting.AmmoCount + "/" + (playerShooting.MaxAmmo + playerShooting.maxAmmoMod);

        expBar.maxValue = playerExpHandler.LevelIncrement;
        expBar.value = playerExpHandler.Exp;
    }
    public void UpdateHealthBar()
    {
        StartCoroutine(AnimateHealthBar());
    }

    private IEnumerator AnimateHealthBar()
    {
        healthBar1.maxValue = player.MaxHealth + player.maxHealthModifier;
        
        var animTime = 0f;

        while (animTime < 1.0f)
        {
            animTime += Time.deltaTime;
            var lerpValue = animTime / 1.0f;
            healthBar1.value = Mathf.Lerp(healthBar1.value, player.CurHealth, lerpValue);
            yield return null;
        }
    }

    public void UpdateAmmoCount()
    {
        ammoTextBar.text = playerShooting.AmmoCount + "/" + (playerShooting.MaxAmmo+ playerShooting.maxAmmoMod);
    }

    public void UpdateExpBar()
    {
        expBar.maxValue = playerExpHandler.LevelIncrement;
        StartCoroutine(AnimateExpBar());
    }

    private IEnumerator AnimateExpBar()
    {
        var animTime = 0f;

        while (animTime < 1.0f)
        {
            animTime += Time.deltaTime;
            var lerpValue = animTime / 1.0f;
            expBar.value = Mathf.Lerp(expBar.value, playerExpHandler.Exp, lerpValue);
            yield return null;
        }
    }
}
