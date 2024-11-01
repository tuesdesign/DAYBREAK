using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Scripts.SettingsMenu
{
    public class BinaryButtonStateHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private int state;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image circleImage;
        [SerializeField] private GameObject circle;
        
        [SerializeField] private ButtonState[] states;
        
        public Action<string> ButtonToggled;

        private void Start()
        {
            SettingStateValues(states[state]);
        }

        public void ButtonSelected()
        {
            if (states.Length == 0)
                return;
            
            ButtonToggled?.Invoke(states[state].value);

            state = (state + 1) % states.Length;
            SettingStateValues(states[state]);
        }

        void SettingStateValues(ButtonState bState)
        {
            backgroundImage.sprite = bState.background;
            circleImage.sprite = bState.circle;
            circle.transform.localPosition = bState.pos;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            backgroundImage.sprite = states[state].background;
            circleImage.sprite = states[state].circle;
            circle.transform.localPosition = states[state].pos;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ButtonSelected();
        }
    }

    [Serializable]
    public class ButtonState
    {
        public string value;
        public Sprite background;
        public Sprite circle;
        public Vector3 pos;
    }
}