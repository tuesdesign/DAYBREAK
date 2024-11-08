using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Scripts
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance => _instance;
        
        [Header("Main UI Items")]
        [SerializeField] private Canvas mainPCUI;
        [SerializeField] private Canvas mainMobileUI;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private Image timerFill;
        [SerializeField] private GameObject countdownText;
    
        [Header("Win/Loss Screen Items")]
        [SerializeField] private Canvas winLossScreen;
        [SerializeField] private TMP_Text winLossText;
        [SerializeField] private TMP_Text timerText;

        private Canvas _activeUI;
    
        private const float StartTime = 300;
        private float _timeValue;
        private bool _countdown;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;
            
            // Determine which UI to display
            _activeUI = SystemInfo.deviceType == DeviceType.Handheld ? mainMobileUI : mainPCUI;

            Time.timeScale = 1;
        }

        private void Start()
        {
            _activeUI.enabled = true;
            _timeValue = StartTime;
            _countdown = true;
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

        public void ReturnCountdown(int time)
        {
            StartCoroutine(Countdown(time));
        }

        private IEnumerator Countdown(int time)
        {
            countdownText.SetActive(true);

            for (int i = time; i > 0; i--)
            {
                countdownText.GetComponent<TMP_Text>().text = i.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }
        
            countdownText.SetActive(false);
            Time.timeScale = 1;
        }

        public void DisplayWinLoss(bool isLoss)
        {
            // Pause game
            Time.timeScale = 0;
            _countdown = false;
            _timeValue = StartTime - _timeValue;
        
            // Display end screen
            _activeUI.enabled = false;
            winLossScreen.enabled = true;

            // Change win/loss text
            winLossText.text = isLoss ? "YOU LOSE" : "YOU WIN";

            // Time alive & display
            float minutes = Mathf.FloorToInt(_timeValue / 60);
            float seconds = Mathf.FloorToInt(_timeValue % 60);
            timerText.text = "You survived: " + $"{minutes:00}:{seconds:00}";
        }
    }
}