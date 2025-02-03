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
        
        private readonly Color _onColor = new Color32(255, 255, 255, 255);
        private readonly Color _offColor = new Color32(125, 125, 125, 255);
        
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
                //background.GetComponent<Image>().color = _offColor;
            }
            else
            {
                LeanTween.moveLocalX(circle, 45.0f, slideDuration).setEaseOutBounce().setOnComplete(ResetClick).setIgnoreTimeScale(true);
                LeanTween.scale(go, Vector3.one, hoverDuration).setIgnoreTimeScale(true);
                circle.GetComponent<Image>().sprite = onImage;
                //background.GetComponent<Image>().color = _onColor;
            }
        }

        private void ResetClick() { _isClick = false; }
    }
}
