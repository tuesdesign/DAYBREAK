using UnityEngine;

namespace UI.Scripts.MainMenu
{
    public class MainMenuAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuCanvas;
        [SerializeField] private GameObject settingsCanvas;
        
        [SerializeField] private float hoverDuration;
        [SerializeField] private float clickDuration;
        [SerializeField] private float scaleDuration;
        
        private bool _isClick;
        
        private static MainMenuAnimator _instance;
        public static MainMenuAnimator Instance => _instance;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            _isClick = false;
        }
        
        public void ButtonHover(GameObject go)
        {
            if (!_isClick)
                LeanTween.scale(go, new Vector3(0.9f, 0.9f, 0.9f), hoverDuration);
        }
        
        public void ButtonExit(GameObject go)
        {
            if (!_isClick)
                LeanTween.scale(go, Vector3.one, hoverDuration);
        }

        public void ButtonClick(GameObject go)
        {
            _isClick = true;
            LeanTween.scale(go, new Vector3(0.85f, 0.85f, 0.85f), clickDuration).setEaseOutBounce();
            LeanTween.scale(go, Vector3.one, clickDuration).setDelay(clickDuration).setOnComplete(ResetClick);
        }

        private void ResetClick() { _isClick = false; }

        public void OpenSettings()
        {
            LeanTween.scale(mainMenuCanvas, Vector3.zero, scaleDuration);
            LeanTween.scale(settingsCanvas, Vector3.one, scaleDuration);
        }
        
        public void CloseSettings()
        {
            LeanTween.scale(mainMenuCanvas, Vector3.one, scaleDuration);
            LeanTween.scale(settingsCanvas, Vector3.zero, scaleDuration);
        }
    }
}
