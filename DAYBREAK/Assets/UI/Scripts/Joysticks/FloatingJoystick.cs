using UnityEngine;

namespace UI.Scripts.Joysticks
{
    public class FloatingJoystick : MonoBehaviour
    {
        [HideInInspector] 
        public RectTransform rectTransform;
        public RectTransform knob;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }
}
