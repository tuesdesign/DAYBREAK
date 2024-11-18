using UnityEngine;

namespace UI.Scripts.MainMenu
{
    public class MainMenuAnimator : Singleton<MainMenuAnimator>
    {
        [SerializeField] private GameObject mainMenuCanvas;
        [SerializeField] private GameObject settingsCanvas;
        
        [SerializeField] private float hoverDuration;
        [SerializeField] private float clickDuration;
        [SerializeField] private float scaleDuration;
        
        private bool _isClick;
        
        public void ButtonHover(GameObject go)
        {
            if (!_isClick)
                LeanTween.scale(go, new Vector3(0.9f, 0.9f, 0.9f), hoverDuration).setIgnoreTimeScale(true);
        }
        
        public void ButtonExit(GameObject go)
        {
            if (!_isClick)
                LeanTween.scale(go, Vector3.one, hoverDuration).setIgnoreTimeScale(true);
        }

        public void ButtonClick(GameObject go)
        {
            _isClick = true;
            LeanTween.scale(go, new Vector3(0.85f, 0.85f, 0.85f), clickDuration).setEaseOutBounce().setIgnoreTimeScale(true);
            LeanTween.scale(go, Vector3.one, clickDuration).setDelay(clickDuration).setOnComplete(ResetClick).setIgnoreTimeScale(true);
        }

        private void ResetClick() { _isClick = false; }

        public void OpenSettings()
        {
            LeanTween.scale(mainMenuCanvas, Vector3.zero, scaleDuration).setIgnoreTimeScale(true);
            LeanTween.scale(settingsCanvas, Vector3.one, scaleDuration).setIgnoreTimeScale(true);
        }
        
        public void CloseSettings()
        {
            LeanTween.scale(mainMenuCanvas, Vector3.one, scaleDuration).setIgnoreTimeScale(true);
            LeanTween.scale(settingsCanvas, Vector3.zero, scaleDuration).setIgnoreTimeScale(true);
        }
    }
}
