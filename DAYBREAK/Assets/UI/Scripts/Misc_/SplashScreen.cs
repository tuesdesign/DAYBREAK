using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Scripts.Misc_
{
    public class SplashScreen : MonoBehaviour
    {
        private void Start()
        {
            Cursor.visible = false;
            StartCoroutine(LoadMainMenu());
        }

        private IEnumerator LoadMainMenu()
        {
            yield return new WaitForSeconds(8);
            SceneManager.LoadScene("UI_MainMenu");
        }
    }
}
