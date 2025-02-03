using UnityEngine;
using UnityEngine.UI;

namespace UI.Scripts.Upgrades
{
    public class LevelUpEffect : MonoBehaviour
    {
        [SerializeField] private Image expBar;
        public bool flash;
        
        void Update()
        {
            expBar.color = flash ? LerpColor(Color.green, Color.white) : Color.white;
        }

        private Color LerpColor(Color firstColor, Color secondColor) => 
            Color.Lerp(firstColor, secondColor, Mathf.Sin(Time.unscaledTime * 15));
    }
}