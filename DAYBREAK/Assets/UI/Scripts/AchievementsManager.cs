using UnityEngine;
using TMPro;

namespace UI.Scripts
{
    public class AchievementsManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text valuesText;

        private void OnEnable()
        {
            UpdateAchievementValues();
        }

        private void UpdateAchievementValues()
        {
            // Determine time survived in mins & secs
            float minutes = Mathf.FloorToInt(PlayerPrefs.GetFloat("TotalTime") / 60);
            float seconds = Mathf.FloorToInt(PlayerPrefs.GetFloat("TotalTime") % 60);
            
            valuesText.text = PlayerPrefs.GetInt("GamesPlayed") + "<br>"
                            + PlayerPrefs.GetInt("GamesWon") + "<br>"
                            + PlayerPrefs.GetInt("GamesLost") + "<br>"
                            + $"{minutes:00}:{seconds:00}" + "<br>"
                            + PlayerPrefs.GetInt("EnemiesKilled") + "<br>"
                            + PlayerPrefs.GetInt("EnemyStatusKills") + "<br>"
                            + PlayerPrefs.GetInt("XpEarned") + "<br>"
                            + PlayerPrefs.GetInt("UpgradesApplied") + "<br>"
                            + PlayerPrefs.GetInt("ShotsFired") + "<br>";
        }
    }
}