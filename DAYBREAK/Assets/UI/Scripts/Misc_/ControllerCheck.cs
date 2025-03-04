using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace UI.Scripts.Misc_
{
    public class ControllerCheck : MonoBehaviour
    {
        public bool controllerConnected;
        private bool _lostFocus;
        
        public static ControllerCheck Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
            
            DontDestroyOnLoad(gameObject);

            if (Gamepad.all.Count > 0)
                controllerConnected = true;
            
            InputSystem.onDeviceChange += OnDeviceChange;
            
            Cursor.visible = controllerConnected != true;
            Cursor.lockState = controllerConnected ? CursorLockMode.Locked : CursorLockMode.None;
        }

        void OnDestroy()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        private void Update()
        {
            if (!Application.isFocused)
                _lostFocus = true;

            if (Application.isFocused && _lostFocus)
            {
                if (Gamepad.all.Count > 0)
                    controllerConnected = true;
            
                Cursor.visible = controllerConnected != true;
                Cursor.lockState = controllerConnected ? CursorLockMode.Locked : CursorLockMode.None;

                _lostFocus = false;
            }
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    controllerConnected = true;
                    break;
                case InputDeviceChange.Disconnected:
                    controllerConnected = false;
                    break;
                case InputDeviceChange.Reconnected:
                    controllerConnected = true;
                    break;
                default:
                    // See InputDeviceChange reference for other event types.
                    break;
            }
            
            Cursor.visible = controllerConnected != true;
            Cursor.lockState = controllerConnected ? CursorLockMode.Locked : CursorLockMode.None;
            
            MenuStateManager.Instance.UpdateState();
        }
    }
}