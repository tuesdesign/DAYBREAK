using TMPro;
using UI.Scripts.Misc_;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UI.Scripts.PauseMenu
{
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField] private Canvas pauseCanvas;
        [SerializeField] private Canvas settingsCanvas;
        
        [SerializeField] private GameObject resumeButton;

        [SerializeField] private TMP_Text timeValueText;
        [SerializeField] private TMP_Text killValueText;
        
        private PlayerIA _playerInputActions;
        private InputAction _pauseMenu;
        
        private bool _isPaused;
        public int killCounter;
        
        private PauseMenuAnimator _animator;
        private UIManager _manager;
        private ControllerCheck _controllerCheck;

        private void Start()
        {
            _animator = FindObjectOfType(typeof(PauseMenuAnimator)) as PauseMenuAnimator;
            _controllerCheck = FindObjectOfType(typeof(ControllerCheck)) as ControllerCheck;
            _manager = FindObjectOfType(typeof(UIManager)) as UIManager;
            killCounter = 0;
        }
        
        private void OnEnable()
        {
            _pauseMenu = _playerInputActions.Game.Pause;
            _pauseMenu.Enable();

            _pauseMenu.performed += PauseGame;
        }
    
        private void OnDisable()
        {
            _pauseMenu.Disable();
        }
        
        private void Awake()
        {
            _playerInputActions = new PlayerIA();
        }

        private void PauseGame(InputAction.CallbackContext ctx)
        {
            _isPaused = !_isPaused;
            
            switch (_isPaused)
            {
                case true:
                    ActivateMenu();
                    break;
                case false when !_animator.inSettings:
                    DeactivateMenu();
                    break;
            }
        }

        private void ActivateMenu()
        {
            Time.timeScale = 0;
            AudioListener.pause = true;

            killValueText.text = killCounter.ToString();
            timeValueText.text = _manager.TimeSurvived();
            
            pauseCanvas.enabled = true;
            settingsCanvas.enabled = true;

            if (_controllerCheck.connected)
                _controllerCheck.SetSelectedButton(resumeButton);
        }

        private void DeactivateMenu()
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
            pauseCanvas.enabled = false;
            settingsCanvas.enabled = false;
        }
    
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("UI_MainMenu");
        }
    }
}