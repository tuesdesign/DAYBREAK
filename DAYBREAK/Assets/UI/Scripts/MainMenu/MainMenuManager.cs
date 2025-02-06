using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Scripts.Misc_;
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

        [Header("Notes")]
        [SerializeField] private GameObject notesScrollList;
        [SerializeField] private GameObject notesTextBackground;
        [SerializeField] private TMP_Text notesText;
        
        private bool _notesOpen;

        private Dictionary<int, string> _notes = new Dictionary<int, string>()
        {
            { 1, "Alectrona-5 (Pre-Devouring)\nClimate: Tidally locked: one side of the planet perpetually faces the local sun, while the other is facing outward into space.\nLand-to-Ocean Ratio: 90:10 (Bodies of water are only found in a narrow strip between the two hemispheres, where the temperature is stable enough for life to persist).\nNotable Features:\nHalf: One hemisphere is an inhospitable desert, with lethal temperatures at all hours. \nAnd Half: The other hemisphere is a frozen wasteland, with permanent ice-age conditions." },
            { 2, "Hello" },
            { 3, "Help" },
        };
        
        [SerializeField] private AutoScrollRect charAutoScrollRect;
        [SerializeField] private AutoScrollRect notesAutoScrollRect;
        
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
        
        // Note Stuff // 

        public void ToggleNotesList()
        {
            _notesOpen = !_notesOpen;

            if (_notesOpen)
            {
                notesScrollList.SetActive(true);
                LeanTween.scaleY(notesScrollList, 1, 0.3f).setIgnoreTimeScale(true);
                notesAutoScrollRect.notesMenuOpen = true;
            }
            else
            {
                StartCoroutine(NotesClose());
                notesAutoScrollRect.notesMenuOpen = false;
            }
        }
        
        private IEnumerator NotesClose()
        {
            LeanTween.scaleY(notesScrollList, 0, 0.3f).setIgnoreTimeScale(true);

            yield return new WaitForSecondsRealtime(0.3f);
            
            notesScrollList.SetActive(true);
        }

        public void OpenNoteText(int noteNum)
        {
            notesTextBackground.SetActive(true);
            notesText.text = _notes[noteNum];
        }

        public void CloseNoteText()
        {
            notesTextBackground.SetActive(false);
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