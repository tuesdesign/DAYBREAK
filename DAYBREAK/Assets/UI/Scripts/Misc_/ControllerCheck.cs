using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI.Scripts.Misc_
{
    public class ControllerCheck : MonoBehaviour
    {
        public bool connected;

        private void Update()
        {
            var controllers = Gamepad.all.Count;
                
            connected = controllers != 0;
        }

        public void SetSelectedButton(GameObject button)
        {
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(button, new BaseEventData(eventSystem));
        }
    }
}