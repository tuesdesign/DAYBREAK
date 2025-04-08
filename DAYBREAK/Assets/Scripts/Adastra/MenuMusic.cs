using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AudioClip Track;
    public AudioSource Source;
    public float Volume = 1f;


    public bool IsEnd = false;
    void Awake()
    {
        Source.clip = Track;
        Source.Play();

        //FindObjectOfType<AdastraTrackControlsBloodMoon>().enabled = false;
        //FindObjectOfType<AdastraTrackControlsDarkDescent>().enabled = false;
    }

    void start()
    {
        Volume = PlayerPrefs.GetFloat("ToggleMusic");
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateVolume();
        Source.volume = Volume;

        if(IsEnd){
            // if there is an AdastraTrackControlsBloodMoon or AdastraTrackControlsDarkDescent in the scene, pause immediately
            if (FindObjectOfType<AdastraTrackControlsBloodMoon>() != null)
            {
                FindObjectOfType<AdastraTrackControlsBloodMoon>().PauseImmediately();
            }
            else if (FindObjectOfType<AdastraTrackControlsDarkDescent>() != null)
            {
                FindObjectOfType<AdastraTrackControlsDarkDescent>().PauseImmediately();
            }
        }

        
    }

    public void UpdateVolume()
    {
        Volume = PlayerPrefs.GetFloat("ToggleMusic", 1f);
    }


}
