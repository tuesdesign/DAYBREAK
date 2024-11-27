using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Scripts.Misc_
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
