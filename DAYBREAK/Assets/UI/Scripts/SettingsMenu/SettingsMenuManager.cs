using UnityEngine;

namespace UI.Scripts.SettingsMenu
{
    public class SettingsMenuManager : MonoBehaviour
    {
        public void ToggleMusic()
        {
            if (PlayerPrefs.GetInt("ToggleMusic") == 0)
                PlayerPrefs.SetInt("ToggleMusic", 1);
            
            else if (PlayerPrefs.GetInt("ToggleMusic") == 1)
                PlayerPrefs.SetInt("ToggleMusic", 0);
        }
        
        public void ToggleSfx()
        {
            if (PlayerPrefs.GetInt("ToggleSfx") == 0)
                PlayerPrefs.SetInt("ToggleSfx", 1);
            
            else if (PlayerPrefs.GetInt("ToggleSfx") == 1)
                PlayerPrefs.SetInt("ToggleSfx", 0);
        }
        
        public void ToggleVibration()
        {
            if (PlayerPrefs.GetInt("ToggleVibration") == 0)
                PlayerPrefs.SetInt("ToggleVibration", 1);
            
            else if (PlayerPrefs.GetInt("ToggleVibration") == 1)
                PlayerPrefs.SetInt("ToggleVibration", 0);
        }
        
        public void ToggleNotif()
        {
            if (PlayerPrefs.GetInt("ToggleNotif") == 0)
                PlayerPrefs.SetInt("ToggleNotif", 1);
            
            else if (PlayerPrefs.GetInt("ToggleNotif") == 1)
                PlayerPrefs.SetInt("ToggleNotif", 0);
        }
    }
}