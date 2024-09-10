using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TMP_CurrentTime : MonoBehaviour
{
    [SerializeField] TMP_Text currentTime;

    void Update()
    {
        currentTime.text = DateTime.Now.ToLongDateString();
    }
}