using System;
using System.Collections;
using TMPro;
using UI.Scripts.Misc_;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;

namespace UI.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [Header("Main UI Items")]
        [SerializeField] private Canvas mainPCUI;
        [SerializeField] private Canvas mainMobileUI;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private Image timerFill;
        
        [Header("Tutorial Items")]
        [SerializeField] private GameObject tutorialPopup;
        [SerializeField] private TMP_Text anyKeyText;
    
        [Header("Win/Loss Screen Items")]
        [SerializeField] public Canvas winLossScreen;
        [SerializeField] private TMP_Text winLossText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private GameObject shareButton;
        [SerializeField] private GameObject copiedText;
        
        private Canvas _activeUI;
    
        public const float StartTime = 300;
        public float timeValue;
        private bool _countdown;
        private bool _tutorialOpen;
        private bool _displayEndScreen;
        
        private ControllerCheck _controllerCheck;

        private void OnEnable()
        {
            InputSystem.onAnyButtonPress.CallOnce(RemoveTutorialPopup);
        }

        private void Awake()
        {
            // Determine which UI to display
            _activeUI = SystemInfo.deviceType == DeviceType.Handheld ? mainMobileUI : mainPCUI;
        }

        private void Start()
        {
            timeValue = StartTime;
            _controllerCheck = FindObjectOfType(typeof(ControllerCheck)) as ControllerCheck;
            
            tutorialPopup.SetActive(true);
            _tutorialOpen = true;
            Time.timeScale = 0;
            
            PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed") + 1);
        }

        private void Update()
        {
            // Don't update time if tutorial popup open
            if (_tutorialOpen) return;
            
            // Main Update Loop //

            switch (timeValue)
            {
                case > 0 when _countdown:
                    timeValue -= Time.deltaTime;
                    PlayerPrefs.SetFloat("TotalTime", PlayerPrefs.GetFloat("TotalTime") + Time.deltaTime);
                    break;

                // Time ran out -> Player wins
                case <= 0 when _displayEndScreen:
                    _displayEndScreen = false;
                    timeValue = 0;
                    DisplayWinLoss(false);
                    break;
            }

            DisplayTime(timeValue);
        }
    
        private void DisplayTime(float timeToDisplay)
        {
            if (timeToDisplay < 0)
                timeToDisplay = 0;

            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            timeText.text = $"{minutes:00}:{seconds:00}";
            timerFill.fillAmount = timeToDisplay / StartTime;
        }

        private void RemoveTutorialPopup(InputControl control)
        {
            if (!_tutorialOpen) return;
            
            tutorialPopup.SetActive(false);
            _activeUI.enabled = true;
            _countdown = true;
            _displayEndScreen = true;
            Time.timeScale = 1;
            
            _tutorialOpen = false;
        }
        
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("UI_MainMenu");
        }
        
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void DisplayWinLoss(bool isLoss)
        {
            // Pause game
            Time.timeScale = 0;
            _countdown = false;
        
            // Display end screen
            _activeUI.enabled = false;
            winLossScreen.enabled = true;
            
            if (_controllerCheck.connected)
                _controllerCheck.SetSelectedButton(shareButton);

            // Change win/loss text
            if (isLoss)
            {
                winLossText.text = "YOU LOSE";
                PlayerPrefs.SetInt("GamesLost", PlayerPrefs.GetInt("GamesLost") + 1);
            }
            else
            {
                winLossText.text = "YOU WIN";
                PlayerPrefs.SetInt("GamesWon", PlayerPrefs.GetInt("GamesWon") + 1);
            }

            // Time alive & display
            timerText.text = "You survived: " + TimeSurvived();
        }

        public string TimeSurvived()
        {
            var tempTime = StartTime - timeValue;
            
            float minutes = Mathf.FloorToInt(tempTime / 60);
            float seconds = Mathf.FloorToInt(tempTime % 60);

            return $"{minutes:00}:{seconds:00}";
        }

        public void CopyText()
        {
            UniClipboard.SetText("I survived " + TimeSurvived() + " in Daybreak today!");
            StartCoroutine(AnimateCopyText());
        }

        IEnumerator AnimateCopyText()
        {
            LeanTween.scale(copiedText, Vector3.one, 0.15f).setIgnoreTimeScale(true);
            yield return new WaitForSecondsRealtime(3);
            LeanTween.scale(copiedText, Vector3.zero, 0.5f).setIgnoreTimeScale(true);
        }
    }
}