using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu_Manager : MonoBehaviour
{
    [Header("Main Menu Items")]
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas pcCharacterSelect;
    [SerializeField] private Canvas mobileCharacterSelect;
    [SerializeField] TMP_Text currentTime;

    private void Awake()
    {
        mainMenu.enabled = true;
        mobileCharacterSelect.enabled = false;
        pcCharacterSelect.enabled = false;
    }

    private void Update()
    {
        currentTime.text = DateTime.Now.ToLongDateString();
    }

    public void OpenCharacterSelect()
    {
        mainMenu.GetComponent<Canvas>().enabled = false;

        if (SystemInfo.deviceType == DeviceType.Handheld)
            mobileCharacterSelect.enabled = true;
        else 
            pcCharacterSelect.enabled = true;
    }
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}