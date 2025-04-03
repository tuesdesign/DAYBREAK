using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdastraTrackControlsBloodMoon : MonoBehaviour
{
    public bool DEBUG = false; // Display debug messages to the Console

    private AudioSource Crash;
    private AudioSource GlitchDrums;
    private AudioSource CleanDrums;
    private AudioSource ClaveRide;
    private AudioSource MelodyA;
    private AudioSource MelodyB;
    private AudioSource MelodyC;
    private AudioSource BackupOrgan;
    private AudioSource Bass;
    private AudioSource ElectronicPad;
    private AudioSource Hihat;
    private AudioSource OrganPad;
    private AudioSource Strings;

    [Range(0.0f, 1.0f)] public float CrashVolume = 0;
    [Range(0.0f, 1.0f)] public float GlitchDrumsVolume = 0;
    [Range(0.0f, 1.0f)] public float CleanDrumsVolume = 0;
    [Range(0.0f, 1.0f)] public float ClaveRideVolume = 0;
    [Range(0.0f, 1.0f)] public float MelodyAVolume = 0;
    [Range(0.0f, 1.0f)] public float MelodyBVolume = 0;
    [Range(0.0f, 1.0f)] public float MelodyCVolume = 0;
    [Range(0.0f, 1.0f)] public float BackupOrganVolume = 0;
    [Range(0.0f, 1.0f)] public float BassVolume = 0;
    [Range(0.0f, 1.0f)] public float ElectronicPadVolume = 0;
    [Range(0.0f, 1.0f)] public float HihatVolume = 0;
    [Range(0.0f, 1.0f)] public float OrganPadVolume = 0;
    [Range(0.0f, 1.0f)] public float StringsVolume = 0;
    
    public int Level = 0;
    public float PlayerHealth = 1f;
    public bool PlayerMoving = false;
    public int BulletsOnScreen = 0;

    public float MASTER_VOLUME = 1f;

    public static AdastraTrackControlsBloodMoon Instance { get; private set; }
    
    public AudioSource FindAudioSource(string name)
    {
        Transform child = transform.Find(name);
        if (child == null)
        {
            Debug.LogError("<color=teal>ADA: </color><color=red>Source Object " + name + " not found</color>");
            return null;
        }
        AudioSource audioSource = child.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("<color=teal>ADA: </color><color=red>Source Component " + name + " not found</color>");
            return null;
        }
        return audioSource;
    }

    void Awake()
    {
        Crash = FindAudioSource("Crash");
        GlitchDrums = FindAudioSource("Glitch Drums");
        CleanDrums = FindAudioSource("Clean Drums");
        ClaveRide = FindAudioSource("Clave Ride");
        MelodyA = FindAudioSource("Melody A");
        MelodyB = FindAudioSource("Melody B");
        MelodyC = FindAudioSource("Melody C");
        BackupOrgan = FindAudioSource("Backup Organ");
        Bass = FindAudioSource("Bass");
        ElectronicPad = FindAudioSource("Electronic Pad");
        Hihat = FindAudioSource("Hihat");
        OrganPad = FindAudioSource("Organ Pad");
        Strings = FindAudioSource("Strings");
        
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        MASTER_VOLUME = PlayerPrefs.GetFloat("ToggleMusic");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Crash.volume = CrashVolume;
        GlitchDrums.volume = GlitchDrumsVolume;
        CleanDrums.volume = CleanDrumsVolume;
        ClaveRide.volume = ClaveRideVolume;
        MelodyA.volume = MelodyAVolume;
        MelodyB.volume = MelodyBVolume;
        MelodyC.volume = MelodyCVolume;
        BackupOrgan.volume = BackupOrganVolume;
        Bass.volume = BassVolume;
        ElectronicPad.volume = ElectronicPadVolume;
        Hihat.volume = HihatVolume;
        OrganPad.volume = OrganPadVolume;
        Strings.volume = StringsVolume;
    }

    public void PauseImmediately()
    {
        Crash.Pause();
        GlitchDrums.Pause();
        CleanDrums.Pause();
        ClaveRide.Pause();
        MelodyA.Pause();
        MelodyB.Pause();
        MelodyC.Pause();
        BackupOrgan.Pause();
        Bass.Pause();
        ElectronicPad.Pause();
        Hihat.Pause();
        OrganPad.Pause();
        Strings.Pause();
    }

    public void UnpauseImmediately()
    {
        Crash.UnPause();
        GlitchDrums.UnPause();
        CleanDrums.UnPause();
        ClaveRide.UnPause();
        MelodyA.UnPause();
        MelodyB.UnPause();
        MelodyC.UnPause();
        BackupOrgan.UnPause();
        Bass.UnPause();
        ElectronicPad.UnPause();
        Hihat.UnPause();
        OrganPad.UnPause();
        Strings.UnPause();
    }

    public float rewindTime = 0.15f;

    public void Rewind()
    {
        Crash.time = Mathf.Max(0, Crash.time - rewindTime);
        GlitchDrums.time = Mathf.Max(0, GlitchDrums.time - rewindTime);
        CleanDrums.time = Mathf.Max(0, CleanDrums.time - rewindTime);
        ClaveRide.time = Mathf.Max(0, ClaveRide.time - rewindTime);
        MelodyA.time = Mathf.Max(0, MelodyA.time - rewindTime);
        MelodyB.time = Mathf.Max(0, MelodyB.time - rewindTime);
        MelodyC.time = Mathf.Max(0, MelodyC.time - rewindTime);
        BackupOrgan.time = Mathf.Max(0, BackupOrgan.time - rewindTime);
        Bass.time = Mathf.Max(0, Bass.time - rewindTime);
        ElectronicPad.time = Mathf.Max(0, ElectronicPad.time - rewindTime);
        Hihat.time = Mathf.Max(0, Hihat.time - rewindTime);
        OrganPad.time = Mathf.Max(0, OrganPad.time - rewindTime);
        Strings.time = Mathf.Max(0, Strings.time - rewindTime);
    }

    public float WindDownTime = 0.15f;
    public float WindUpTime = 0.15f;

    bool wind = false;
    public void ToggleWindEffect()
    {
        if (wind)
        {
            WindDown();
        }
        else
        {
            WindUp();
        }
        wind = !wind;
    }

    public void WindDown()
    {
        StartCoroutine(WindDownCoroutine());
    }

    public void WindUp()
    {
        StartCoroutine(WindUpCoroutine());
    }

    private IEnumerator WindDownCoroutine()
    {
        if (DEBUG) { Debug.Log("<color=teal>ADA: </color><color=green>Effect 'slowdown' START</color>"); } // DEBUG message
        // over the course of WindDownTime, reduce pitch to 0, using only realtime fuctions to avoid issues with 0 time scale
        float startTime = Time.realtimeSinceStartup;
        float endTime = startTime + WindDownTime;
        float startPitch = Crash.pitch;
        float v = 1f;
        while (Time.realtimeSinceStartup < endTime)
        {
            float t = (Time.realtimeSinceStartup - startTime) / WindDownTime;
            v = Mathf.Lerp(startPitch, 0, t);
            Crash.pitch = v;
            GlitchDrums.pitch = v;
            CleanDrums.pitch = v;
            ClaveRide.pitch = v;
            MelodyA.pitch = v;
            MelodyB.pitch = v;
            MelodyC.pitch = v;
            BackupOrgan.pitch = v;
            Bass.pitch = v;
            ElectronicPad.pitch = v;
            Hihat.pitch = v;
            OrganPad.pitch = v;
            Strings.pitch = v;
            //Add additional channels here...
            yield return null;
        }
        //pause immediately
        PauseImmediately();
    }

    private IEnumerator WindUpCoroutine()
    {
        // rewind
        //Rewind();
        // unpause, then over the course of WindUpTime, increase pitch to 1
        UnpauseImmediately();
        float startTime = Time.realtimeSinceStartup;
        float endTime = startTime + WindUpTime;
        float startPitch = Crash.pitch;
        float v = 0f;
        while (Time.realtimeSinceStartup < endTime)
        {
            float t = (Time.realtimeSinceStartup - startTime) / WindUpTime;
            v = Mathf.Lerp(startPitch, 1, t);
            Crash.pitch = v;
            GlitchDrums.pitch = v;
            CleanDrums.pitch = v;
            ClaveRide.pitch = v;
            MelodyA.pitch = v;
            MelodyB.pitch = v;
            MelodyC.pitch = v;
            BackupOrgan.pitch = v;
            Bass.pitch = v;
            ElectronicPad.pitch = v;
            Hihat.pitch = v;
            OrganPad.pitch = v;
            Strings.pitch = v;
            //Add additional channels here...
            yield return null;
        }

        if (DEBUG) { Debug.Log("<color=teal>ADA: </color><color=green>Effect 'slowdown' END</color>"); } // DEBUG message
    }

    void Update()
    {
        // Crash
        // increase by 0.2 for each bullet on screen (0-1)
        CrashVolume = Mathf.Clamp01(BulletsOnScreen * 0.2f) * MASTER_VOLUME; 
        
        // GlitchDrums
        // approach 1 as player health approaches 0
        GlitchDrumsVolume = Mathf.Clamp01(1 - PlayerHealth) * MASTER_VOLUME;
        
        // CleanDrums
        // approach 1 as player health approaches 1
        CleanDrumsVolume = Mathf.Clamp01(PlayerHealth) * MASTER_VOLUME;
        

        // ClaveRide
        // increase when player is moving, decrease when player is not moving
        if (PlayerMoving)
        {
            ClaveRideVolume = Mathf.Clamp01(ClaveRideVolume + Time.deltaTime) * MASTER_VOLUME;
        }
        else
        {
            ClaveRideVolume = Mathf.Clamp01(ClaveRideVolume - Time.deltaTime) * MASTER_VOLUME;
        }

        // MelodyA
        // increase when player is moving, decrease when player is not moving
        if (PlayerMoving)
        {
            MelodyAVolume = Mathf.Clamp01(MelodyAVolume + Time.deltaTime/3) * MASTER_VOLUME;
        }
        else
        {
            MelodyAVolume = Mathf.Clamp01(MelodyAVolume - Time.deltaTime/3) * MASTER_VOLUME;
        }
        // MelodyB
        // increase when player is moving, decrease when player is not moving
        if (PlayerMoving)
        {
            MelodyBVolume = Mathf.Clamp01(MelodyBVolume + Time.deltaTime/3) * MASTER_VOLUME;
        }
        else
        {
            MelodyBVolume = Mathf.Clamp01(MelodyBVolume - Time.deltaTime/3) * MASTER_VOLUME;
        }
        // MelodyC
        // increase when player is moving, decrease when player is not moving
        if (PlayerMoving)
        {
            MelodyCVolume = Mathf.Clamp01(MelodyCVolume + Time.deltaTime/3) * MASTER_VOLUME;
        }
        else
        {
            MelodyCVolume = Mathf.Clamp01(MelodyCVolume - Time.deltaTime/3) * MASTER_VOLUME;
        }
        // BackupOrgan
        // set to 1
        BackupOrganVolume = 1 * MASTER_VOLUME;

        // Bass
        // decrease when player is moving, increase when player is not moving
        if (PlayerMoving)
        {
            BassVolume = Mathf.Clamp01(BassVolume - Time.deltaTime/15) * MASTER_VOLUME;
        }
        else
        {
            BassVolume = Mathf.Clamp01(BassVolume + Time.deltaTime/3) * MASTER_VOLUME;
        }
        
        // ElectronicPad
        // increase by 0.1 for each level (0-1)
        ElectronicPadVolume = Mathf.Clamp01(Level * 0.1f) * MASTER_VOLUME;
        
        // Hihat
        // increase when player is moving, decrease when player is not moving (slow)
        if (PlayerMoving)
        {
            HihatVolume = Mathf.Clamp01(HihatVolume + Time.deltaTime/10) * MASTER_VOLUME;
        }
        else
        {
            HihatVolume = Mathf.Clamp01(HihatVolume - Time.deltaTime) * MASTER_VOLUME;
        }

        // OrganPad
        // increase by 0.1 for each level (0-1)
        OrganPadVolume = Mathf.Clamp01(Level * 0.1f) * MASTER_VOLUME;

        // Strings
        // approach 1 as player health approaches 0
        StringsVolume = Mathf.Clamp01(1 - PlayerHealth) * MASTER_VOLUME;
    }

    public void SetLevel(int level)
    {
        //Level = level;
    }
    public void SetPlayerHealth(float health)
    {
        PlayerHealth = health;
    }
    public void SetPlayerMoving(bool moving)
    {
        PlayerMoving = moving;
    }
    public void SetBulletsOnScreen(int bullets)
    {
        BulletsOnScreen = bullets;
    }

    public void SetMasterVolume(float volume)
    {
        MASTER_VOLUME = volume;
    }

}
