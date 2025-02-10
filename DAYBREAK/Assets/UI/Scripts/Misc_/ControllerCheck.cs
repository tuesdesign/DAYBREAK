using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Scripts.Misc_
{
    public class ControllerCheck : MonoBehaviour
    {
        public bool controllerConnected;
        
        public static ControllerCheck Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
            
            DontDestroyOnLoad(gameObject);
            
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        void OnDestroy()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
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
            
            MenuStateManager.Instance.UpdateState();
        }
    }
}