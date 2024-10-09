using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas pcCharacterSelect;
    [SerializeField] private Canvas mobileCharacterSelect;
    
    [SerializeField] TMP_Text currentTime;

    private void Awake()
    {
        mainMenu.GetComponent<Canvas>().enabled = true;
        pcCharacterSelect.GetComponent<Canvas>().enabled = false;
        mobileCharacterSelect.GetComponent<Canvas>().enabled = false;
    }

    void Update()
    {
        currentTime.text = DateTime.Now.ToLongDateString();
    }

    public void OpenCharacterSelect()
    {
        mainMenu.GetComponent<Canvas>().enabled = false;

        if (SystemInfo.deviceType == DeviceType.Handheld)
            mobileCharacterSelect.GetComponent<Canvas>().enabled = true;
        else 
            pcCharacterSelect.GetComponent<Canvas>().enabled = true;
    }
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
