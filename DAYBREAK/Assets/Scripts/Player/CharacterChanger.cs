using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterChanger : MonoBehaviour
{
    PlayerBase playerBase;
    UpgradeHandling upgradeHandling;
    public PlayerSO characterSelected;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game") 
        {
            GameSceneUpdater(characterSelected);
        }
    }
    public void ChangeCharacter(PlayerSO character)
    {

        characterSelected = character;
    }

    void GameSceneUpdater(PlayerSO character)
    {
        playerBase = FindAnyObjectByType<PlayerBase>().GetComponent<PlayerBase>();
        upgradeHandling = FindAnyObjectByType<UpgradeHandling>().GetComponent<UpgradeHandling>();
        playerBase.ApplyCharacterStats(character);
        upgradeHandling.ApplyUpgrade(character.baseUpgradables);
    }
}