using UnityEngine;

namespace UI.Scripts.MainMenu
{
    public class MainMenuAnimator : MonoBehaviour
    {
        [SerializeField] private float hoverDuration;
        [SerializeField] private float clickDuration;
        
        private bool _isClick;
        
        public void ButtonHover(GameObject go)
        {
            if (!_isClick)
                LeanTween.scale(go, new Vector3(1.1f, 1.1f, 1.1f), hoverDuration).setIgnoreTimeScale(true);
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
    }
}