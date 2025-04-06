using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CharacterChanger : MonoBehaviour
{
    PlayerBase playerBase;
    UpgradeHandling upgradeHandling;

    public PlayerSO characterSelected;


    //private void Awake()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //private void OnDestroy()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game") 
        {
            Debug.Log("Scene Changed!!");
            StartCoroutine(CharacterFinder());
            //GameSceneUpdater(characterSelected);
        }
    }
    public void ChangeCharacter(PlayerSO character)
    {

        characterSelected = character;
        StartCoroutine(CharacterFinder());
    }

    public void GameSceneUpdater(PlayerSO character)
    {
        playerBase = FindAnyObjectByType<PlayerBase>();
        upgradeHandling = FindAnyObjectByType<UpgradeHandling>();
        playerBase.ApplyCharacterStats(character);
        //playerBase.characterApplied = true;

        upgradeHandling.ApplyUpgrade(character.baseUpgradables);
        //Debug.Log("Updated to " + characterSelected.name);

        characterSelected = null;
    }

    IEnumerator CharacterFinder()
    {
          
        yield return new WaitUntil(() => FindAnyObjectByType<PlayerBase>() != null);
        GameSceneUpdater(characterSelected);
    }
}