using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.Scripts.Upgrades
{
    public class UpgradeManagerMenu : MonoBehaviour
    {
        [Header("Upgrade 1")]
        [SerializeField] private TMP_Text titleText1;
        [SerializeField] private TMP_Text descriptionText1;
    
        [Header("Upgrade 2")]
        [SerializeField] private TMP_Text titleText2;
        [SerializeField] private TMP_Text descriptionText2;
    
        [Header("Upgrade 3")]
        [SerializeField] private TMP_Text titleText3;
        [SerializeField] private TMP_Text descriptionText3;
    
        [SerializeField] private GameObject upgradeHandler;
        private UpgradeHandling _upgradeObject;

        private UpgradeBaseSO _upgrade1;
        private UpgradeBaseSO _upgrade2;
        private UpgradeBaseSO _upgrade3;
        private bool _appliedUpgrade;
    
        private LevelUpEffect _flashEffect;

        private void Awake()
        {
            _upgradeObject = upgradeHandler.GetComponent<UpgradeHandling>();
            _flashEffect = GetComponent<LevelUpEffect>();
        }
        
        public void PopulateMenu()
        {
            var index = Random.Range(0, _upgradeObject.FullupgradeList.Count - 1);

            titleText1.text = _upgradeObject.FullupgradeList[index].upgradeName;
            descriptionText1.text = _upgradeObject.FullupgradeList[index].description;
            _upgrade1 = _upgradeObject.FullupgradeList[index];
        
            var temp1 = index;
            while (temp1 == index)
                index = Random.Range(0, _upgradeObject.FullupgradeList.Count - 1);
            
            titleText2.text = _upgradeObject.FullupgradeList[index].upgradeName;
            descriptionText2.text = _upgradeObject.FullupgradeList[index].description;
            _upgrade2 = _upgradeObject.FullupgradeList[index];
        
            var temp2 = index;
            while (temp1 == index || temp2 == index)
                index = Random.Range(0, _upgradeObject.FullupgradeList.Count - 1);
            
            titleText3.text = _upgradeObject.FullupgradeList[index].upgradeName;
            descriptionText3.text = _upgradeObject.FullupgradeList[index].description;
            _upgrade3 = _upgradeObject.FullupgradeList[index];
            
            _flashEffect.flash = true;
            _appliedUpgrade = false;
            
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.UpgradeState);
            GameObject.Find("ada_track_alex1").GetComponent<AdastraDemoTrackControlsAlexTrack1>().WindDown(); // absolutely hideous function call, replace plz
            Time.timeScale = 0;
        }

        public void ApplyUpgrade(int buttonNumber)
        {
            if (!_appliedUpgrade)
            {
                switch (buttonNumber)
                {
                    case 1:
                        _upgradeObject.ApplyUpgrade(_upgrade1);
                        break;
                    case 2:
                        _upgradeObject.ApplyUpgrade(_upgrade2);
                        break;
                    case 3:
                        _upgradeObject.ApplyUpgrade(_upgrade3);
                        break;
                }
            
                _appliedUpgrade = true;
            }
            
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.GameplayState);
            _flashEffect.flash = false;
            Time.timeScale = 1;
            GameObject.Find("ada_track_alex1").GetComponent<AdastraDemoTrackControlsAlexTrack1>().WindUp(); // absolutely hideous function call, replace plz
            
            // Add Impulse force for player
        }
    }
}