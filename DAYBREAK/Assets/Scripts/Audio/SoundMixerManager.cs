using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class SoundMixerManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        private float _sfxVolume, _musicVolume;
        public void SetMusicVolume(float level)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
        }
        
        public void SetSoundFXVolume(float level)
        {
            audioMixer.SetFloat("SfxVolume", Mathf.Log10(level) * 20f);
        }
        
        public void ToggleMusicVolume()
        {
            audioMixer.GetFloat("MusicVolume", out _musicVolume);

            if (_musicVolume == 0)
                audioMixer.SetFloat("MusicVolume", -80);
            else 
                audioMixer.SetFloat("MusicVolume", 0);
        }
        
        public void ToggleSoundFXVolume()
        {
            audioMixer.GetFloat("SfxVolume", out _sfxVolume);

            if (_sfxVolume == 0)
                audioMixer.SetFloat("SfxVolume", -80);
            else 
                audioMixer.SetFloat("SfxVolume", 0);
        }
    }
}