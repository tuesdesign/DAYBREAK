using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private PlayerIA _playerInputActions;
    private InputAction _pauseMenu;
    
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private bool isPaused;

    private void Awake()
    {
        _playerInputActions = new PlayerIA();
    }

    private void PauseGame(InputAction.CallbackContext ctx)
    {
        isPaused = !isPaused;

        if (isPaused)
            ActivateMenu();
        else
            DeactivateMenu();
        
    }

    private void ActivateMenu()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        pauseCanvas.enabled = true;
    }
    
    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        pauseCanvas.enabled = false;
        isPaused = false;
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("UI_MainMenu");
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
}