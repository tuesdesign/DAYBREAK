using UI.Scripts.Misc_;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Scripts.PauseMenu
{
    public class PauseMenuAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject settingsCanvas;
        [SerializeField] private GameObject musicButton;
        [SerializeField] private GameObject resumeButton;
        
        [SerializeField] private float hoverDuration;
        [SerializeField] private float clickDuration;
        [SerializeField] private float scaleDuration;
        
        private bool _isClick;
        public bool inSettings;
        
        private ControllerCheck _controllerCheck;

        private void Start()
        {
            _controllerCheck = FindObjectOfType(typeof(ControllerCheck)) as ControllerCheck;
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

            if (_controllerCheck.connected)
            {
                var eventSystem = EventSystem.current;
                eventSystem.SetSelectedGameObject(musicButton, new BaseEventData(eventSystem));
            }
        }
        
        public void CloseSettings()
        {
            LeanTween.scale(settingsCanvas, Vector3.zero, scaleDuration).setIgnoreTimeScale(true);
            inSettings = false;
            
            if (_controllerCheck.connected)
            {
                var eventSystem = EventSystem.current;
                eventSystem.SetSelectedGameObject(resumeButton, new BaseEventData(eventSystem));
            }
        }
    }
}