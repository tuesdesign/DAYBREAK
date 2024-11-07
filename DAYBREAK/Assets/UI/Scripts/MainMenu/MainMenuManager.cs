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
        [SerializeField] private Canvas mainMenu;
        [SerializeField] private Canvas characterSelect;
        [SerializeField] private TMP_Text currentTime;

        private void Awake()
        {
            mainMenu.enabled = true;
            characterSelect.enabled = false;
            Time.timeScale = 1;
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
            characterSelect.enabled = true;
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