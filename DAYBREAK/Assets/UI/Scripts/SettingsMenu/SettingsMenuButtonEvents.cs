using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Scripts.SettingsMenu
{
    public enum SettingState
    {
        On,
        Off
    }
    
    public class SettingsMenuButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject circleImage;
        
        private SettingState _settingState;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            SettingsMenuAnimator.Instance.ButtonHover(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SettingsMenuAnimator.Instance.ButtonExit(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (_settingState)
            {
                case SettingState.On:
                    SettingsMenuAnimator.Instance.ButtonClick(circleImage, true);
                    _settingState = SettingState.Off;
                    break;
                case SettingState.Off:
                    SettingsMenuAnimator.Instance.ButtonClick(circleImage, false);
                    _settingState = SettingState.On;
                    break;
            }
        }
    }
}
