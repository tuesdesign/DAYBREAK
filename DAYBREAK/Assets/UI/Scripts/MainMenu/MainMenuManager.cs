using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Scripts.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Main Menu Items")]
        [SerializeField] private TMP_Text currentTimePC;
        [SerializeField] private TMP_Text currentTimeMobile;
        [SerializeField] private GameObject loadingText;

        [Header("PC/Mobile Differences")]
        [SerializeField] public GameObject pcTitleObject;
        [SerializeField] public GameObject mobileTitleObject;
        [SerializeField] public GameObject quitButton;
        
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
            currentTimePC.text = DateTime.Now.ToLongDateString();
            currentTimeMobile.text = DateTime.Now.ToLongDateString();

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