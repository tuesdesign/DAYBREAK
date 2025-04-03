using UnityEngine;
using UnityEngine.UI;

namespace UI.Scripts.SettingsMenu
{
    public class SettingsMenuManager : MonoBehaviour
    {
        private PlayerShooting _playerShooting;
        
        private void Start()
        {
            PlayerPrefs.GetFloat("ToggleMusic", 1.0f);
            PlayerPrefs.GetFloat("ToggleSfx", 1.0f);
            PlayerPrefs.GetInt("ToggleTwinStick", 1);
            PlayerPrefs.GetInt("ToggleVibration", 1);
            PlayerPrefs.GetInt("ToggleNotif", 1);
            PlayerPrefs.GetInt("MouseAim", 1);

            _playerShooting = FindObjectOfType<PlayerShooting>();
        }

        public void ToggleMusic(Slider slider)
        {
            PlayerPrefs.SetFloat("ToggleMusic", slider.value);
            
            if (AdastraTrackControlsBloodMoon.Instance != null)
                AdastraTrackControlsBloodMoon.Instance.MASTER_VOLUME = slider.value;
        
            if (AdastraTrackControlsDarkDescent.Instance != null) 
                AdastraTrackControlsDarkDescent.Instance.MASTER_VOLUME = slider.value;
            
            PlayerPrefs.Save();
        }

        public void MinMaxMusic()
        {
            if (PlayerPrefs.GetFloat("ToggleMusic") == 0)
            {
                PlayerPrefs.SetFloat("ToggleMusic", 1);
                
                if (AdastraTrackControlsBloodMoon.Instance != null)
                    AdastraTrackControlsBloodMoon.Instance.MASTER_VOLUME = 1;
        
                if (AdastraTrackControlsDarkDescent.Instance != null) 
                    AdastraTrackControlsDarkDescent.Instance.MASTER_VOLUME = 1;
            }
            else if (PlayerPrefs.GetFloat("ToggleMusic") == 1)
            {
                PlayerPrefs.SetFloat("ToggleMusic", 0);
                
                if (AdastraTrackControlsBloodMoon.Instance != null)
                    AdastraTrackControlsBloodMoon.Instance.MASTER_VOLUME = 0;
        
                if (AdastraTrackControlsDarkDescent.Instance != null) 
                    AdastraTrackControlsDarkDescent.Instance.MASTER_VOLUME = 0;
            }
            Debug.Log(PlayerPrefs.GetFloat("ToggleMusic"));
            
            PlayerPrefs.Save();
        }
        
        public void ToggleSfx(Slider slider)
        {
            PlayerPrefs.SetFloat("ToggleSfx", slider.value);
            
            PlayerPrefs.Save();
        }

        public void MinMaxSfx()
        {
            if (PlayerPrefs.GetFloat("ToggleSfx") == 0)
                PlayerPrefs.SetFloat("ToggleSfx", 1);
            
            else if (PlayerPrefs.GetFloat("ToggleSfx") == 1)
                PlayerPrefs.SetFloat("ToggleSfx", 0);
            
            PlayerPrefs.Save();
        }
        
        public void ToggleTwinStick()
        {
            if (PlayerPrefs.GetInt("ToggleTwinStick") == 0)
                PlayerPrefs.SetInt("ToggleTwinStick", 1);
            
            else if (PlayerPrefs.GetInt("ToggleTwinStick") == 1)
                PlayerPrefs.SetInt("ToggleTwinStick", 0);
            
            PlayerPrefs.Save();
            
            if (_playerShooting != null)
                _playerShooting.CheckTwinstick();
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
        
        public void ToggleMouseAim()
        {
            if (PlayerPrefs.GetInt("MouseAim") == 0)
                PlayerPrefs.SetInt("MouseAim", 1);
            
            else if (PlayerPrefs.GetInt("MouseAim") == 1)
                PlayerPrefs.SetInt("MouseAim", 0);
            
            PlayerPrefs.Save();
            
            if (_playerShooting != null)
                _playerShooting.CheckMouseAim();
        }
    }
}