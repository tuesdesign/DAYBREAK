using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

namespace UI.Scripts.Misc_
{
    public class SplashScreen : MonoBehaviour
    {
        private void OnEnable()
        {
            InputSystem.onAnyButtonPress.CallOnce(SkipScene);
        }
        
        private void Start()
        {
            Cursor.visible = false;
            StartCoroutine(LoadMainMenu());
        }

        private void SkipScene(InputControl control)
        {
            SceneManager.LoadScene("UI_MainMenu");
        }
        
        private IEnumerator LoadMainMenu()
        {
            yield return new WaitForSeconds(8.5f);
            SceneManager.LoadScene("UI_MainMenu");
        }
    }
}