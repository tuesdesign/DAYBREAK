using TMPro;
using UI.Scripts.Misc_;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI.Scripts.PauseMenu
{
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text timeValueText;
        [SerializeField] private TMP_Text killValueText;
        
        private PlayerIA _playerInputActions;
        private InputAction _pauseMenu;
        
        private bool _isPaused;
        private bool _inSettings;
        public int killCounter;
        
        private UIManager _manager;

        private void Start()
        {
            _manager = FindObjectOfType(typeof(UIManager)) as UIManager;
            killCounter = 0;

            _isPaused = false;
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
            Pause();
        }

        public void Pause()
        {
            if (MenuStateManager.Instance.CurrentState == MenuStateManager.Instance.WinLossState || MenuStateManager.Instance.CurrentState == MenuStateManager.Instance.UpgradeState) return;
            
            _isPaused = !_isPaused;
            
            switch (_isPaused)
            {
                case true:
                    ActivateMenu();
                    break;
                case false when !_inSettings:
                    DeactivateMenu();
                    break;
            }
        }

        private void ActivateMenu()
        {
            GameObject.Find("ada_track_alex1").GetComponent<AdastraDemoTrackControlsAlexTrack1>().WindDown(); // absolutely hideous function call, replace plz
            Time.timeScale = 0;
            //AudioListener.pause = true;

            killValueText.text = killCounter.ToString();
            timeValueText.text = _manager.TimeSurvived();
            
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.PauseState);
        }

        private void DeactivateMenu()
        {
            Time.timeScale = 1;
            GameObject.Find("ada_track_alex1").GetComponent<AdastraDemoTrackControlsAlexTrack1>().WindUp(); // absolutely hideous function call, replace plz
            //AudioListener.pause = false;
            
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.GameplayState);
        }
        
        public void OpenSettings()
        {
            _inSettings = true;
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.GameplaySettingsState);
        }
        
        public void CloseSettings()
        {
            _inSettings = false;
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.PauseState);
        }
    
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("UI_MainMenu");
        }
    }
}