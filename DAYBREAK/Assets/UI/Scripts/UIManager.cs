using System.Collections;
using TMPro;
using UI.Scripts.Misc_;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace UI.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [Header("Main UI Items")]
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private Image timerFill;
        
        [Header("Tutorial Items")]
        [SerializeField] private GameObject tutorialPopup;
    
        [Header("Win/Loss Screen Items")]
        [SerializeField] private TMP_Text winLossText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private GameObject copiedText;
        
        [Header("Canvases")] 
        [SerializeField] public GameObject gameplayUI;
        [SerializeField] public GameObject upgradeMenu;
        [SerializeField] public GameObject pauseMenu;
        [SerializeField] public GameObject settingsMenu;
        [SerializeField] public GameObject winLossMenu;
        
        [Header("Primary Buttons")]
        [SerializeField] public GameObject upgradeMenuPrimary;
        [SerializeField] public GameObject pauseMenuPrimary;
        [SerializeField] public GameObject settingsMenuPrimary;
        [SerializeField] public GameObject winLossMenuPrimary;
    
        public const float StartTime = 300;
        private float _timeValue;
        private bool _countdown;
        private bool _tutorialOpen;
        private bool _displayEndScreen;
        
        public static UIManager Instance { get; private set; }
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
            
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.GameplayState);
        }
        
        private void OnEnable()
        {
            InputSystem.onAnyButtonPress.CallOnce(RemoveTutorialPopup);
        }

        private void Start()
        {
            _timeValue = StartTime;
            
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

            switch (_timeValue)
            {
                case > 0 when _countdown:
                    _timeValue -= Time.deltaTime;
                    PlayerPrefs.SetFloat("TotalTime", PlayerPrefs.GetFloat("TotalTime") + Time.deltaTime);
                    break;

                // Time ran out -> Player wins
                case <= 0 when _displayEndScreen:
                    _displayEndScreen = false;
                    _timeValue = 0;
                    DisplayWinLoss(false);
                    break;
            }

            DisplayTime(_timeValue);
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
            _countdown = true;
            _displayEndScreen = true;
            Time.timeScale = 1;
            
            _tutorialOpen = false;
        }

        // Win Loss Screen //
        
        public void DisplayWinLoss(bool isLoss)
        {
            // Pause game
            Time.timeScale = 0;
            _countdown = false;

            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.WinLossState);
            
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
            var tempTime = StartTime - _timeValue;
            
            float minutes = Mathf.FloorToInt(tempTime / 60);
            float seconds = Mathf.FloorToInt(tempTime % 60);

            return $"{minutes:00}:{seconds:00}";
        }

        // Win Loss Buttons //
        
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("UI_MainMenu");
        }
        
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
            MenuStateManager.Instance.ForceExitState();
            //MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.GameplayState);
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