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

    [Tooltip("This is the health bar.")] [SerializeField]
    Slider healthBar1;

    [Tooltip("This is the exp bar.")] [SerializeField]
    Slider expBar;

    [Tooltip("The text bar to describe how much ammo the player has in comparison to their max ammo")] [SerializeField]
    TMP_Text ammoTextBar;

    [Header("Ammo Display")]
    [SerializeField] private GameObject pistolAmmoPrefab;
    [SerializeField] private GameObject ammoDisplayHolder;
    private List<Image> _ammoMainImages = new List<Image>();

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

        InitialAmmoDisplay();
        
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

    public void UpdateAmmoDisplayRemove(int bulletsShot)
    {
        StartCoroutine(DelayedAmmoUIRemove(bulletsShot));
    }

    private IEnumerator DelayedAmmoUIRemove(int bulletsShot)
    {
        yield return new WaitForEndOfFrame();

        for (var i = 1; i <= bulletsShot; i++)
        {
            var numAmmo = (playerShooting.MaxAmmo + playerShooting.maxAmmoMod) - (playerShooting.AmmoCount + i);

            if (numAmmo >= 0 && numAmmo < _ammoMainImages.Count)
                _ammoMainImages[numAmmo].enabled = false;
        }
    }
    
    public void UpdateAmmoDisplayAdd()
    {
        var numAmmo = (playerShooting.MaxAmmo + playerShooting.maxAmmoMod) - (playerShooting.AmmoCount + 1);

        if (numAmmo >= 0 && numAmmo < _ammoMainImages.Count)
            _ammoMainImages[numAmmo].enabled = true;
    }
   
    // Add initial number of bullets displayed on UI
    private void InitialAmmoDisplay()
    {
        // Create UI for ammo count
        for (int i = 0; i < playerShooting.MaxAmmo + playerShooting.maxAmmoMod; i++)
        {
            Instantiate(pistolAmmoPrefab, ammoDisplayHolder.transform);
        }

        foreach (Transform child in ammoDisplayHolder.transform)
        {
            _ammoMainImages.Add(child.transform.GetChild(1).GetComponent<Image>());
        }
    }
    
    // Add UI for max ammo mod count
    public void UpdateAmmoDisplay()
    {
        for (var i = 0; i < playerShooting.maxAmmoMod; i++)
        {
            var newUI = Instantiate(pistolAmmoPrefab, ammoDisplayHolder.transform);
            _ammoMainImages.Add(newUI.transform.GetChild(1).GetComponent<Image>());
        }
        
        playerShooting.ForceReload();
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
