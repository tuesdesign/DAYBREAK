using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Scripts.SettingsMenu
{
    public enum SettingState
    {
        On,
        Off
    }
    
    public class SettingsMenuButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [SerializeField] private GameObject circleImage;
        [SerializeField] private GameObject backgroundImage;
        [SerializeField] private string settingPlayerPref;
        [SerializeField] private bool isSlider;
        
        private SettingState _settingState;
        private int _settingInt;
        
        private SettingsMenuAnimator _animator;

        private void Start()
        {
            _animator = FindObjectOfType(typeof(SettingsMenuAnimator)) as SettingsMenuAnimator;
            UpdateButton();
        }

        private void Awake()
        {
            _settingInt = PlayerPrefs.GetInt(settingPlayerPref);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isSlider) return;
            _animator.ButtonHover(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isSlider) return;
            _animator.ButtonExit(gameObject);
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isSlider) return;
            
            switch (_settingState)
            {
                case SettingState.On:
                    _animator.ButtonClick(gameObject, circleImage, backgroundImage, true);
                    _settingState = SettingState.Off;
                    break;
                case SettingState.Off:
                    _animator.ButtonClick(gameObject, circleImage, backgroundImage, false);
                    _settingState = SettingState.On;
                    break;
            }
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            if (isSlider) return;
            
            _animator.ButtonHover(gameObject);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (isSlider) return;
            
            _animator.ButtonExit(gameObject);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (isSlider) return;
            
            switch (_settingState)
            {
                case SettingState.On:
                    _animator.ButtonClick(gameObject, circleImage, backgroundImage, true);
                    _settingState = SettingState.Off;
                    break;
                case SettingState.Off:
                    _animator.ButtonClick(gameObject, circleImage, backgroundImage, false);
                    _settingState = SettingState.On;
                    break;
            }
        }

        private void UpdateButton()
        {
            if (isSlider)
            {
                GetComponent<Slider>().value = PlayerPrefs.GetFloat(settingPlayerPref);
                return;
            }
            
            switch (_settingInt)
            {
                case 0:
                    _animator.ButtonClick(gameObject, circleImage, backgroundImage, true);
                    _settingState = SettingState.Off;
                    break;
                case 1:
                    _animator.ButtonClick(gameObject, circleImage,backgroundImage, false);
                    _settingState = SettingState.On;
                    break;
            }
        }
    }
}
