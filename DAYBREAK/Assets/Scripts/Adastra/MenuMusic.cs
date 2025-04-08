using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MenuMusic : MonoBehaviour
{
    public AudioClip track;
    public AudioSource source;
    public bool isEnd;

    public void Awake()
    {
        source.clip = track;
        source.Play();
    }

    private void Update()
    {
        if (!isEnd) return;
        
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