using System;
using System.Collections;
using TMPro;
using UI.Scripts.Misc_;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace UI.Scripts.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Main Menu Items")]
        [SerializeField] private Canvas mainMenu;
        [SerializeField] private Canvas characterSelect;
        [SerializeField] private TMP_Text currentTime;
        
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject character1Button;

        private AutoScrollRect _autoScrollRect;
        
        private ControllerCheck _controllerCheck;
        private bool _existingController;

        private void Awake()
        {
            mainMenu.enabled = true;
            characterSelect.enabled = false;
        }
        
        private void Start()
        {
            _autoScrollRect = FindObjectOfType(typeof(AutoScrollRect)) as AutoScrollRect;
            _controllerCheck = FindObjectOfType(typeof(ControllerCheck)) as ControllerCheck;
        }

        private void Update()
        {
            currentTime.text = DateTime.Now.ToLongDateString();

            if (_existingController) return;
            
            if (mainMenu.enabled && _controllerCheck.connected)
            {
                _controllerCheck.SetSelectedButton(playButton);
                _existingController = true;
            }
                
            if (characterSelect.enabled && _controllerCheck.connected)
            {
                _controllerCheck.SetSelectedButton(character1Button);
                _existingController = true;
            }
        }

        public void OpenCharacterSelect()
        {
            StartCoroutine(CharacterSelectOpen());
        }
        
        public void CloseCharacterSelect()
        {
            StartCoroutine(CharacterSelectClose());
        }

        private IEnumerator CharacterSelectOpen()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            
            mainMenu.enabled = false;
            characterSelect.enabled = true;
            _autoScrollRect.menuOpen = true;

            if (_controllerCheck.connected)
            {
                _controllerCheck.SetSelectedButton(character1Button);
                _existingController = true;
            }
        }
        
        private IEnumerator CharacterSelectClose()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            
            mainMenu.enabled = true;
            characterSelect.enabled = false;
            _autoScrollRect.menuOpen = false;

            if (_controllerCheck.connected)
            {
                _controllerCheck.SetSelectedButton(playButton);
                _existingController = true;
            }
        }
    
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadGame(sceneName));
            _autoScrollRect.menuOpen = false;
        }

        private IEnumerator LoadGame(string sceneName)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            SceneManager.LoadScene(sceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}