using UnityEngine;

namespace UI.Scripts.PauseMenu
{
    public class PauseMenuAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject settingsCanvas;
        
        [SerializeField] private float hoverDuration;
        [SerializeField] private float clickDuration;
        [SerializeField] private float scaleDuration;
        
        private bool _isClick;
        public bool inSettings;
        
        private static PauseMenuAnimator _instance;
        public static PauseMenuAnimator Instance => _instance;
        
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
            LeanTween.scale(settingsCanvas, Vector3.one, scaleDuration).setIgnoreTimeScale(true);
            inSettings = true;
        }
        
        public void CloseSettings()
        {
            LeanTween.scale(settingsCanvas, Vector3.zero, scaleDuration).setIgnoreTimeScale(true);
            inSettings = false;
        }
    }
}