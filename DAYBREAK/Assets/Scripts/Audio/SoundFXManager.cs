using UnityEngine;

namespace Audio
{
    public class SoundFXManager : MonoBehaviour
    {
        public static SoundFXManager Instance;

        [SerializeField] private AudioSource soundFXObject;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
        {
            // spawn in gameObject
            AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

            // assign the audioClip
            audioSource.clip = audioClip;

            // assign volume
            audioSource.volume = volume;

            // play sound
            audioSource.Play();
            
            // get length of sound FX clip
            var clipLength = audioSource.clip.length;

            // destroy the clip after it is done playing
            Destroy(audioSource.gameObject, clipLength);
        }
    }
}