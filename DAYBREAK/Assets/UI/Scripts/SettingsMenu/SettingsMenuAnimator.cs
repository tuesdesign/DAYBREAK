using System;
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
        
        private bool _isClick;
            
        private static SettingsMenuAnimator _instance;
        public static SettingsMenuAnimator Instance => _instance;
        
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
                LeanTween.scale(go, new Vector3(0.95f, 0.95f, 0.95f), hoverDuration);
        }
            
        public void ButtonExit(GameObject go)
        {
            if (!_isClick)
                LeanTween.scale(go, Vector3.one, hoverDuration);
        }
    
        public void ButtonClick(GameObject go, bool slideLeft)
        {
            _isClick = true;

            if (slideLeft)
            {
                LeanTween.moveLocalX(go, -45.0f, slideDuration).setEaseOutBounce().setOnComplete(ResetClick);
                go.GetComponent<Image>().sprite = offImage;
            }
            else
            {
                LeanTween.moveLocalX(go, 45.0f, slideDuration).setEaseOutBounce().setOnComplete(ResetClick);
                go.GetComponent<Image>().sprite = onImage;
            }
        }

        private void ResetClick() { _isClick = false; }
    }
}
