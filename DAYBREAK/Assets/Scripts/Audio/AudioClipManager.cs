using UnityEngine;

namespace Audio
{
    public class AudioClipManager : MonoBehaviour
    {
        [Header("UI SFX")] 
        [SerializeField] public AudioClip hoverSoundClip;
        [SerializeField] public AudioClip smallSelectSoundClip;
        [SerializeField] public AudioClip largeSelectSoundClip;
        
        [Header("Gameplay SFX")] 
        [SerializeField] public AudioClip[] hurtSounds;
        [SerializeField] public AudioClip pickupSound;
        [SerializeField] public AudioClip burnSfx;
        [SerializeField] public AudioClip freezeSfx;
        [SerializeField] public AudioClip slowSfx;
        [SerializeField] public AudioClip poisonSfx;
        [SerializeField] public AudioClip shieldUpSfx;
        [SerializeField] public AudioClip shieldDownSfx;
        
        [Header("Shoot Sfx")]
        [SerializeField] public AudioClip[] shootSounds;
        [SerializeField] public AudioClip reloadStartSound;
        [SerializeField] public AudioClip reloadSound;
        [SerializeField] public AudioClip reloadStopSound;
        
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