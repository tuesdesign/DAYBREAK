using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdastraDemoTrackControlsAlexTrack1 : MonoBehaviour
{
    /////////////////////////////
    /// Track Initialization  ///
    /////////////////////////////

    public bool DEBUG = false; // Display debug messages to the Console

    private GameObject Source1; // The object containing the audio source
    //Add additional channels here...

    private void Awake() // Get source objects at start of runtime, before execution
    {
        Source1 = GameObject.Find("ada_src1"); // Find Playback Channel 1
        if (DEBUG && Source1 == null) { Debug.LogError("<color=teal>ADA: </color><color=red>Source 1 not found</color>"); } // ERR handler
        //Add additional channels here...
    }
        
    
    private void Start() // Start playback - ensure all channels start at the same time
    {
        Source1.GetComponent<AudioSource>().Play(); // Start playback on Channel 1
        if (DEBUG) { Debug.Log("<color=teal>ADA: </color><color=green>Playback started on Channel 1</color>"); } // DEBUG message 
        //Add additional channels here...
    }

    //////////////////////////////////////////
    /// Generic Playback Control Functions ///
    //////////////////////////////////////////

    public void PauseImmediately()
    {
        Source1.GetComponent<AudioSource>().Pause();
        if (DEBUG) { Debug.Log("<color=teal>ADA: </color><color=yellow>Playback PAUSED on Channel 1</color>"); } // DEBUG message
        //Add additional channels here...
    }
    public void UnpauseImmediately()
    {
        Source1.GetComponent<AudioSource>().UnPause();
        if (DEBUG) { Debug.Log("<color=teal>ADA: </color><color=yellow>Playback UN-PAUSED on Channel 1</color>"); } // DEBUG message
        //Add additional channels here...
    }

    public float rewindTime = 0.15f;

    public void Rewind()
    {
        // Set the playback position to the current playback position minus the rewind time
        Source1.GetComponent<AudioSource>().time = Mathf.Max(0, Source1.GetComponent<AudioSource>().time - rewindTime);
        if (DEBUG) { Debug.Log("<color=teal>ADA: </color><color=orange>Playback on Channel 1 REWOUND by "+rewindTime.ToString()+"s</color>"); } // DEBUG message
        //Add additional channels here...
        if (DEBUG) { Debug.Log(Source1.GetComponent<AudioSource>().time); } // ERR handler
    }


    /////////////////////////////////////////////////
    /// Track-Specific Playback Control Functions ///
    /////////////////////////////////////////////////
    
    // this song is just one track... a bit overkill

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
        float startPitch = Source1.GetComponent<AudioSource>().pitch;
        while (Time.realtimeSinceStartup < endTime)
        {
            float t = (Time.realtimeSinceStartup - startTime) / WindDownTime;
            Source1.GetComponent<AudioSource>().pitch = Mathf.Lerp(startPitch, 0, t);
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
        float startPitch = Source1.GetComponent<AudioSource>().pitch;
        while (Time.realtimeSinceStartup < endTime)
        {
            float t = (Time.realtimeSinceStartup - startTime) / WindUpTime;
            Source1.GetComponent<AudioSource>().pitch = Mathf.Lerp(startPitch, 1, t);
            //Add additional channels here...
            yield return null;
        }

        if (DEBUG) { Debug.Log("<color=teal>ADA: </color><color=green>Effect 'slowdown' END</color>"); } // DEBUG message
    }
}
