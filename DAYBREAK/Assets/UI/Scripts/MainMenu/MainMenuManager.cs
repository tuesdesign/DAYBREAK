using System;
using System.Collections;
using TMPro;
using UI.Scripts.Notes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Scripts.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Main Menu Items")]
        [SerializeField] private TMP_Text currentTime;
        [SerializeField] private GameObject loadingText;
        
        private NotesManager _notesManager;

        [Header("Canvases")] 
        [SerializeField] public GameObject mainMenu;
        [SerializeField] public GameObject settingsMenu;
        [SerializeField] public GameObject achievementsMenu;
        [SerializeField] public GameObject notesMenu;
        [SerializeField] public GameObject characterSelect;
        
        [Header("Primary Buttons")] 
        [SerializeField] public GameObject mainMenuPrimary;
        [SerializeField] public GameObject settingsMenuPrimary;
        [SerializeField] public GameObject achievementsMenuPrimary;
        [SerializeField] public GameObject notesMenuPrimary;
        [SerializeField] public GameObject characterSelectPrimary;

        public static MainMenuManager Instance { get; private set; }
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }
        
        private void Start()
        {
            currentTime.text = DateTime.Now.ToLongDateString();

            MenuStateManager.Instance.forcedExit = true;
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.MainMenuState);

            loadingText.SetActive(false);
        }
        
        // Character Select Page //

        public void OpenCharacterSelect()
        {
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.CharacterSelectState);
        }
        
        public void CloseCharacterSelect()
        {
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.MainMenuState);
        }
        
        
        // Achievements Page //

        public void OpenAchievementsPage()
        {
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.AchievementsState);
        }

        public void CloseAchievementsPage()
        {
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.MainMenuState);
        }
        
        
        // Settings Stuff //
        
        public void OpenSettings()
        {
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.MainSettingsState);
        }
        
        public void CloseSettings()
        {
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.MainMenuState);
        }

        
    
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadGame(sceneName));
        }

        private IEnumerator LoadGame(string sceneName)
        {
            MenuStateManager.Instance.ForceExitState();
            loadingText.SetActive(true);
            
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene(sceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}