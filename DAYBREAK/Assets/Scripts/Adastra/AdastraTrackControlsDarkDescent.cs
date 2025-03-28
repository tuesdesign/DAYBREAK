using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdastraTrackControlsDarkDescent : MonoBehaviour
{
    // Hello! If you're reading this, you prbaby want to know how to make it work.
    // Please use the SetLevel, SetPlayerHealth, SetPlayerMoving, and SetBulletsOnScreen functions to control the music.
    // The music will automatically adjust based on the values you set.


    /////////////////////////////
    /// Track Initialization  ///
    /////////////////////////////

    public bool DEBUG = false; // Display debug messages to the Console

    private AudioSource GuitarSource;
    private AudioSource StringPulseSource;
    private AudioSource CowbellSource;
    private AudioSource RideSource;
    private AudioSource BassSource;
    private AudioSource ChoirSource;
    private AudioSource FranticPluckSource;
    private AudioSource GlitchOrganSource;
    private AudioSource IntroSource;
    private AudioSource LeadSource;
    private AudioSource MainOrganSource;

    [Range(0, 1)] public float GuitarVolume = 0;
    [Range(0, 1)] public float StringPulseVolume = 0;
    [Range(0, 1)] public float CowbellVolume = 0;
    [Range(0, 1)] public float RideVolume = 0;
    [Range(0, 1)] public float BassVolume = 0;
    [Range(0, 1)] public float ChoirVolume = 0;
    [Range(0, 1)] public float FranticPluckVolume = 0;
    [Range(0, 1)] public float GlitchOrganVolume = 0;
    [Range(0, 1)] public float IntroVolume = 0;
    [Range(0, 1)] public float LeadVolume = 0;
    [Range(0, 1)] public float MainOrganVolume = 0;

    public int Level = 1;
    public float PlayerHealth = 1;
    public bool PlayerMoving = false;
    public int BulletsOnScreen = 0;
    

    // Start is called before the first frame update
    private void Awake()
    {
        // Find Stems (children of this game object)
        GuitarSource = FindAudioSource("Guitar"); // Add 0.1 for every Level
        StringPulseSource = FindAudioSource("String Pulse"); // 1 minus Player health percentage
        CowbellSource = FindAudioSource("Cowbell"); // 0 if stationary, 1 if moving
        RideSource = FindAudioSource("Ride"); // Reduce if stationary, increase if moving
        BassSource = FindAudioSource("Bass"); // Keep at 1
        ChoirSource = FindAudioSource("Choir"); // Increase if stationary, reduce if moving (slowly)
        FranticPluckSource = FindAudioSource("Frantic Pluck");
        GlitchOrganSource = FindAudioSource("Glitch Organ"); // increase by 0.2 for every bullet on screen
        IntroSource = FindAudioSource("Intro Drums"); // keep at 1
        LeadSource = FindAudioSource("Lead Pluck"); // Keep at 1
        MainOrganSource = FindAudioSource("Main Organ"); // increase by 0.1 each time a bullet is fired
    }
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

    // Update is called once per frame
    void LateUpdate()
    {
        // Set volume of each stem
        GuitarSource.volume = GuitarVolume;
        StringPulseSource.volume = StringPulseVolume;
        CowbellSource.volume = CowbellVolume;
        RideSource.volume = RideVolume;
        BassSource.volume = BassVolume;
        ChoirSource.volume = ChoirVolume;
        FranticPluckSource.volume = FranticPluckVolume;
        GlitchOrganSource.volume = GlitchOrganVolume;
        IntroSource.volume = IntroVolume;
        LeadSource.volume = LeadVolume;
        MainOrganSource.volume = MainOrganVolume;
    }

    void Update()
    {
        // Guitar (Add 0.1 for every Level, clamp at 0-1)
        GuitarVolume = Mathf.Clamp((0.1f * Level), 0, 1);

        // String Pulse (1 minus Player health percentage, clamp at 0-1)
        StringPulseVolume = Mathf.Clamp(1 - PlayerHealth, 0, 1);

        // Cowbell (0 if stationary, 1 if moving)
        CowbellVolume = PlayerMoving ? 1 : 0;

        // Ride (Reduce if stationary, increase if moving)
        RideVolume = PlayerMoving ? Mathf.Min(1, RideVolume + 0.001f) : Mathf.Max(0, RideVolume - 0.001f);

        // Bass (Keep at 1)
        BassVolume = 1;

        // Choir (Increase if stationary, reduce if moving (slowly))
        ChoirVolume = PlayerMoving ? Mathf.Max(0, ChoirVolume - 0.001f) : Mathf.Min(1, ChoirVolume + 0.001f);

        // Frantic Pluck (Keep at 1)
        FranticPluckVolume = 1;

        // Glitch Organ (increase by 0.2 for every bullet on screen, clamp at 0-1)
        GlitchOrganVolume = Mathf.Clamp(0.2f * BulletsOnScreen, 0, 1);

        // Intro Drums (keep at 1)
        IntroVolume = 1;

        // Lead Pluck (Keep at 1)
        LeadVolume = 1;

        // Main Organ (increase by 0.1 each bullet, clamp at 0-1)
        MainOrganVolume = Mathf.Clamp(0.1f * BulletsOnScreen, 0, 1);
    }

    public void SetLevel(int level)
    {
        Level = level;
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


}
