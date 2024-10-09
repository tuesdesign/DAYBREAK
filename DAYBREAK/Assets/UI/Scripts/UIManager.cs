using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu Items")]
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas pcCharacterSelect;
    [SerializeField] private Canvas mobileCharacterSelect;
    [SerializeField] TMP_Text currentTime;
    
    [Header("Win/Loss Screen Items")]
    [SerializeField] private Canvas winLossScreen;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image timerFill;
    
    private const float StartTime = 300;
    private float _timeValue;
    private bool updateTimer = false;

    private void Awake()
    {
        if (mainMenu != null)
            mainMenu.GetComponent<Canvas>().enabled = true;
        if (pcCharacterSelect != null)
            pcCharacterSelect.GetComponent<Canvas>().enabled = false;
        if (mobileCharacterSelect != null)
            mobileCharacterSelect.GetComponent<Canvas>().enabled = false;

        if (timerText != null)
            updateTimer = true;
    }
    
    private void Start()
    {
        _timeValue = StartTime;
    }

    void Update()
    {
        if (!timerText)
            currentTime.text = DateTime.Now.ToLongDateString();
        
        if (_timeValue > 0)
        {
            _timeValue -= Time.deltaTime;
        }
        else
        {
            // Time Ran Out
            _timeValue = 0;
        }
        
        if (updateTimer)
            DisplayTime(_timeValue);
    }
    
    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerFill.fillAmount = timeToDisplay / StartTime;
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

    public void DisplayWinLoss(float time)
    {
        winLossScreen.GetComponent<Canvas>().enabled = true;
    }
}
