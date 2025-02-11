using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace UI.Scripts.Upgrades
{
    public class LevelUpEffect : MonoBehaviour
    {
        [SerializeField] private Image expBar;
        
        public bool flash;

        private float _flashSpeed;
        
        void Update()
        {
            expBar.color = flash ? LerpColor(Color.green, Color.white) : Color.white;
        }

        private Color LerpColor(Color firstColor, Color secondColor) => 
            Color.Lerp(firstColor, secondColor, Mathf.Sin(Time.unscaledTime * 15));
        
        public void PulseColor(Color firstColor, Color secondColor)
        {
            StartCoroutine(Pulse(firstColor, secondColor));
        }

        IEnumerator Pulse(Color firstColor, Color secondColor)
        {
            float time = 0;

            while (time < 0.2f)
            {
                expBar.color = Color.Lerp(firstColor, secondColor, time / 0.2f);
                time += Time.deltaTime;
                yield return null;
            }

            expBar.color = secondColor;
            
            time = 0;
            
            while (time < 0.2f)
            {
                expBar.color = Color.Lerp(secondColor, firstColor, time / 0.2f);
                time += Time.deltaTime;
                yield return null;
            }

            expBar.color = firstColor;
        }
    }
}