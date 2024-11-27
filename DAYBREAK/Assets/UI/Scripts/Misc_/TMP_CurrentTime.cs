using System;
using TMPro;
using UnityEngine;

namespace UI.Scripts.Misc_
{
    public class TMP_CurrentTime : MonoBehaviour
    {
        [SerializeField] TMP_Text currentTime;

        void Update()
        {
            currentTime.text = DateTime.Now.ToLongDateString();
        }
    }
}