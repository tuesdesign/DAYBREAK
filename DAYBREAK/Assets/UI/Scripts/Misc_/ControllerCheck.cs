using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI.Scripts.Misc_
{
    public class ControllerCheck : MonoBehaviour
    {
        public bool connected;

        private void Awake() {
            StartCoroutine(CheckForControllers());
        }

        private IEnumerator CheckForControllers() 
        {
            while (true) 
            {
                var controllers = Gamepad.all.Count;

                connected = connected switch
                {
                    false when controllers > 0 => true,
                    true when controllers == 0 => false,
                    _ => connected
                };

                yield return new WaitForSeconds(15f);
            }
        }

        public void SetSelectedButton(GameObject button)
        {
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(button, new BaseEventData(eventSystem));
        }
    }
}