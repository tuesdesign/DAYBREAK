using System;
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
        [SerializeField] private string settingPlayerPref;
        
        private SettingState _settingState;
        private int _settingInt;

        private void Awake()
        {
            _settingInt = PlayerPrefs.GetInt(settingPlayerPref);
        }

        private void Start()
        {
            UpdateButton();
        }

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

        private void UpdateButton()
        {
            switch (_settingInt)
            {
                case 0:
                    SettingsMenuAnimator.Instance.ButtonClick(circleImage, true);
                    _settingState = SettingState.Off;
                    break;
                case 1:
                    SettingsMenuAnimator.Instance.ButtonClick(circleImage, false);
                    _settingState = SettingState.On;
                    break;
            }
        }
    }
}
