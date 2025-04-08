using UnityEngine;

namespace Audio
{
    public class AudioClipManager : MonoBehaviour
    {
        [Header("Sound FX")] 
        [SerializeField] public AudioClip hoverSoundClip;
        [SerializeField] public AudioClip smallSelectSoundClip;
        [SerializeField] public AudioClip largeSelectSoundClip;
        
        public static AudioClipManager Instance { get; private set; }
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        
            DontDestroyOnLoad(this.gameObject);
        }
    } 
}