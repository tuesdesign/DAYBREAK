using System.Collections;
using TMPro;
using UI.Scripts.Misc_;
using UI.Scripts.PauseMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [Header("Main UI Items")]
        [SerializeField] private Canvas mainPCUI;
        [SerializeField] private Canvas mainMobileUI;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private Image timerFill;
    
        [Header("Win/Loss Screen Items")]
        [SerializeField] private Canvas winLossScreen;
        [SerializeField] private TMP_Text winLossText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private GameObject shareButton;
        [SerializeField] private GameObject copiedText;
        
        private Canvas _activeUI;
    
        private const float StartTime = 300;
        private float _timeValue;
        private bool _countdown;
        
        private ControllerCheck _controllerCheck;

        private void Awake()
        {
            // Determine which UI to display
            _activeUI = SystemInfo.deviceType == DeviceType.Handheld ? mainMobileUI : mainPCUI;

            Time.timeScale = 1;
        }

        private void Start()
        {
            _activeUI.enabled = true;
            _timeValue = StartTime;
            _countdown = true;
            _controllerCheck = FindObjectOfType(typeof(ControllerCheck)) as ControllerCheck;
        }

        private void Update()
        {
            switch (_timeValue)
            {
                case > 0 when _countdown:
                    _timeValue -= Time.deltaTime;
                    break;
                
                // Time ran out -> Player wins
                case <= 0:
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
            {
                var eventSystem = EventSystem.current;
                eventSystem.SetSelectedGameObject(shareButton, new BaseEventData(eventSystem));
            }

            // Change win/loss text
            winLossText.text = isLoss ? "YOU LOSE" : "YOU WIN";

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