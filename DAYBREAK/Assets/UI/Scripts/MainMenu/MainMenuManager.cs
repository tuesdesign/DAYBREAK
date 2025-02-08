using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Scripts.Misc_;
using UI.Scripts.Notes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Scripts.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Main Menu Items")]
        [SerializeField] private Canvas mainMenu;
        [SerializeField] private Canvas characterSelect;
        [SerializeField] private TMP_Text currentTime;
        
        [Header("Buttons")]
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject character1Button;
        
        [Header("Achievements UI")]
        [SerializeField] private GameObject achievementCanvas;
        [SerializeField] private GameObject achievementBackButton;
        
        [SerializeField] private AutoScrollRect charAutoScrollRect;
        
        private NotesManager _notesManager;
        private ControllerCheck _controllerCheck;
        private bool _existingController;

        private void Awake()
        {
            mainMenu.enabled = true;
            characterSelect.enabled = false;
        }
        
        private void Start()
        {
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
        
        // Achievements Page //

        public void OpenAchievementsPage()
        {
            achievementCanvas.SetActive(true);
            _controllerCheck.SetSelectedButton(achievementBackButton);
        }

        public void CloseAchievementsPage()
        {
            achievementCanvas.SetActive(false);
            _controllerCheck.SetSelectedButton(playButton);
        }
        
        
        // Character Select Stuff //

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
            charAutoScrollRect.menuOpen = true;

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
            charAutoScrollRect.menuOpen = false;

            if (_controllerCheck.connected)
            {
                _controllerCheck.SetSelectedButton(playButton);
                _existingController = true;
            }
        }
    
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadGame(sceneName));
            charAutoScrollRect.menuOpen = false;
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