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
    
    [Header("Main UI Items")]
    [SerializeField] private Canvas mainPCUI;
    [SerializeField] private Canvas mainMobileUI;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Image timerFill;
    
    [Header("Win/Loss Screen Items")]
    [SerializeField] private Canvas winLossScreen;
    [SerializeField] private TMP_Text winLossText;
    [SerializeField] private TMP_Text timerText;
    
    
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
        
        if (_timeValue > 0 && updateTimer)
        {
            _timeValue -= Time.deltaTime;
        }
        // Time ran out -> Player wins
        else if (updateTimer)
        {
            _timeValue = 0;
            DisplayWinLoss(false);
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

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("UI_MainMenu");
    }


    public void DisplayWinLoss(bool isLoss)
    {
        // Pause game
        Time.timeScale = 0;
        
        // Stop timer and display end screen 
        updateTimer = false;
        mainPCUI.GetComponent<Canvas>().enabled = false;
        mainMobileUI.GetComponent<Canvas>().enabled = false;
        winLossScreen.GetComponent<Canvas>().enabled = true;

        // Change win/loss text
        if (isLoss)
            winLossText.text = "YOU LOSE";
        if (!isLoss)
            winLossText.text = "YOU WIN";

        // Convert time left -> time alive & display
        _timeValue = StartTime - _timeValue;
        float minutes = Mathf.FloorToInt(_timeValue / 60);
        float seconds = Mathf.FloorToInt(_timeValue % 60);
        timerText.text = "You survived: " + $"{minutes:00}:{seconds:00}";
    }
}