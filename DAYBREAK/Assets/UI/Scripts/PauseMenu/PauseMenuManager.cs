using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI.Scripts.PauseMenu
{
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField] private Canvas pauseCanvas;
        [SerializeField] private Canvas settingsCanvas;
        
        private PlayerIA _playerInputActions;
        private InputAction _pauseMenu;
        
        private bool _isPaused;
        
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
                case false when !PauseMenuAnimator.Instance.inSettings:
                    DeactivateMenu();
                    break;
            }
        }

        private void ActivateMenu()
        {
            Time.timeScale = 0;
            AudioListener.pause = true;
            pauseCanvas.enabled = true;
            settingsCanvas.enabled = true;
        }
    
        public void DeactivateMenu()
        {
            UIManager.Instance.ReturnCountdown(3);
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