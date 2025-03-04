using System;
using UnityEngine;

namespace UI.Scripts.SettingsMenu
{
    public class SettingsMenuManager : MonoBehaviour
    {
        private void Start()
        {
            PlayerPrefs.GetInt("ToggleMusic", 1);
            PlayerPrefs.GetInt("ToggleSfx", 1);
            PlayerPrefs.GetInt("ToggleTwinStick", 1);
            PlayerPrefs.GetInt("ToggleVibration", 1);
            PlayerPrefs.GetInt("ToggleNotif", 1);
        }

        public void ToggleMusic()
        {
            if (PlayerPrefs.GetInt("ToggleMusic") == 0)
                PlayerPrefs.SetInt("ToggleMusic", 1);
            
            else if (PlayerPrefs.GetInt("ToggleMusic") == 1)
                PlayerPrefs.SetInt("ToggleMusic", 0);
            
            PlayerPrefs.Save();
        }
        
        public void ToggleSfx()
        {
            if (PlayerPrefs.GetInt("ToggleSfx") == 0)
                PlayerPrefs.SetInt("ToggleSfx", 1);
            
            else if (PlayerPrefs.GetInt("ToggleSfx") == 1)
                PlayerPrefs.SetInt("ToggleSfx", 0);
            
            PlayerPrefs.Save();
        }
        
        public void ToggleTwinStick()
        {
            if (PlayerPrefs.GetInt("ToggleTwinStick") == 0)
                PlayerPrefs.SetInt("ToggleTwinStick", 1);
            
            else if (PlayerPrefs.GetInt("ToggleTwinStick") == 1)
                PlayerPrefs.SetInt("ToggleTwinStick", 0);
            
            PlayerPrefs.Save();
        }
        
        public void ToggleVibration()
        {
            if (PlayerPrefs.GetInt("ToggleVibration") == 0)
                PlayerPrefs.SetInt("ToggleVibration", 1);
            
            else if (PlayerPrefs.GetInt("ToggleVibration") == 1)
                PlayerPrefs.SetInt("ToggleVibration", 0);
            
            PlayerPrefs.Save();
        }
        
        public void ToggleNotif()
        {
            if (PlayerPrefs.GetInt("ToggleNotif") == 0)
                PlayerPrefs.SetInt("ToggleNotif", 1);
            
            else if (PlayerPrefs.GetInt("ToggleNotif") == 1)
                PlayerPrefs.SetInt("ToggleNotif", 0);
            
            PlayerPrefs.Save();
        }
    }
}