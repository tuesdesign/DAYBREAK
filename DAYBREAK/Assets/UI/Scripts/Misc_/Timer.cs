using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Scripts.Misc_
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private Image timerFill;

        private const float StartTime = 300;
        private float _timeValue;

        private void Start()
        {
            _timeValue = StartTime;
        }

        void Update()
        {
            if (_timeValue > 0)
                _timeValue -= Time.deltaTime;
            else // Time ran out
                _timeValue = 0;
        
            DisplayTime(_timeValue);
        }

        private void DisplayTime(float timeToDisplay)
        {
            if (timeToDisplay < 0)
                timeToDisplay = 0;

            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            timerText.text = $"{minutes:00}:{seconds:00}";
            timerFill.fillAmount = timeToDisplay / StartTime;
        }
    }
}