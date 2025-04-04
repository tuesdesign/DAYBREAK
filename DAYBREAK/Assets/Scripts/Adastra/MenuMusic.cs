using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AudioClip Track;
    public AudioSource Source;
    public float Volume = 1f;

    void Awake()
    {
        Source.clip = Track;
        Source.Play();

        FindObjectOfType<AdastraTrackControlsBloodMoon>().enabled = false;
        FindObjectOfType<AdastraTrackControlsDarkDescent>().enabled = false;
    }

    void start()
    {
        Source.volume = PlayerPrefs.GetFloat("ToggleMusic", 1f);
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateVolume();
        Source.volume = Volume;
    }

    public void UpdateVolume()
    {
        Volume = PlayerPrefs.GetFloat("ToggleMusic", 1f);
    }
}
