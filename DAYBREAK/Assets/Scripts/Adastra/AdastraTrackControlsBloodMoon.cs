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
    
    public int level = 0;
    public float PlayerHealth = 1f;
    public bool PlayerMoving = false;
    public int BulletsOnScreen = 0;

    public float MASTER_VOLUME = 1f;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        //CrashVolume = 1 * MASTER_VOLUME;
        // GlitchDrums
        //GlitchDrumsVolume = 1 * MASTER_VOLUME;
        // CleanDrums
        //CleanDrumsVolume = 1 * MASTER_VOLUME;
        // ClaveRide
        //ClaveRideVolume = 1 * MASTER_VOLUME;
        // MelodyA
        //MelodyAVolume = 1 * MASTER_VOLUME;
        // MelodyB
        //MelodyBVolume = 1 * MASTER_VOLUME;
        // MelodyC
        //MelodyCVolume = 1 * MASTER_VOLUME;
        // BackupOrgan
        //BackupOrganVolume = 1 * MASTER_VOLUME;
        // Bass
        //BassVolume = 1 * MASTER_VOLUME;
        // ElectronicPad
        //ElectronicPadVolume = 1 * MASTER_VOLUME;
        // Hihat
        //HihatVolume = 1 * MASTER_VOLUME;
        // OrganPad
        //OrganPadVolume = 1 * MASTER_VOLUME;
        // Strings
        //StringsVolume = 1 * MASTER_VOLUME;
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
