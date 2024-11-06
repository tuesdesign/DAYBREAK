using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Scripts.MainMenu
{
    public class MainMenu_Manager : MonoBehaviour
    {
        [Header("Main Menu Items")]
        [SerializeField] private Canvas mainMenu;
        [SerializeField] private Canvas pcCharacterSelect;
        [SerializeField] private Canvas mobileCharacterSelect;
        [SerializeField] TMP_Text currentTime;

        private static MainMenu_Manager _instance;

        public static MainMenu_Manager Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(this.gameObject);
            else
                _instance = this;
        
            mainMenu.enabled = true;
            mobileCharacterSelect.enabled = false;
            pcCharacterSelect.enabled = false;
        }

        private void Update()
        {
            currentTime.text = DateTime.Now.ToLongDateString();
        }

        public void OpenCharacterSelect()
        {
            StartCoroutine(CharacterSelect());
        }

        private IEnumerator CharacterSelect()
        {
            yield return new WaitForSeconds(0.2f);
            
            mainMenu.enabled = false;

            if (SystemInfo.deviceType == DeviceType.Handheld)
                mobileCharacterSelect.enabled = true;
            else 
                pcCharacterSelect.enabled = true;
        }
    
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadGame(sceneName));
        }

        private IEnumerator LoadGame(string sceneName)
        {
            yield return new WaitForSeconds(0.2f);
            SceneManager.LoadScene(sceneName);
        }
    }
}