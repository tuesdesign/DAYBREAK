using UnityEngine;
using UnityEngine.UI;

namespace UI.Scripts.SettingsMenu
{
    public class SettingsMenuAnimator : MonoBehaviour
    {
        [SerializeField] private float hoverDuration;
        [SerializeField] private float slideDuration;
        [SerializeField] private Sprite onImage;
        [SerializeField] private Sprite offImage;
        [SerializeField] private Sprite onBgImage;
        [SerializeField] private Sprite offBgImage;
        
        private bool _isClick;
            
        public void ButtonHover(GameObject go)
        {
            if (!_isClick)
                LeanTween.scale(go, new Vector3(0.95f, 0.95f, 0.95f), hoverDuration).setIgnoreTimeScale(true);
        }
            
        public void ButtonExit(GameObject go)
        {
            if (!_isClick)
                LeanTween.scale(go, Vector3.one, hoverDuration).setIgnoreTimeScale(true);
        }
    
        public void ButtonClick(GameObject go, GameObject circle, GameObject background, bool slideLeft)
        {
            _isClick = true;

            if (slideLeft)
            {
                LeanTween.moveLocalX(circle, -45.0f, slideDuration).setEaseOutBounce().setOnComplete(ResetClick).setIgnoreTimeScale(true);
                LeanTween.scale(go, Vector3.one, hoverDuration).setIgnoreTimeScale(true);
                circle.GetComponent<Image>().sprite = offImage;
                background.GetComponent<Image>().sprite = offBgImage;
            }
            else
            {
                LeanTween.moveLocalX(circle, 45.0f, slideDuration).setEaseOutBounce().setOnComplete(ResetClick).setIgnoreTimeScale(true);
                LeanTween.scale(go, Vector3.one, hoverDuration).setIgnoreTimeScale(true);
                circle.GetComponent<Image>().sprite = onImage;
                background.GetComponent<Image>().sprite = onBgImage;
            }
        }

        private void ResetClick() { _isClick = false; }
    }
}
