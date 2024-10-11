using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class UpgradeManagerMenu : MonoBehaviour
{
    [SerializeField] private Canvas upgradeMenu;
    [SerializeField] private TMP_Text descriptionText1;
    [SerializeField] private Image image1;
    [SerializeField] private TMP_Text descriptionText2;
    [SerializeField] private Image image2;
    [SerializeField] private TMP_Text descriptionText3;
    [SerializeField] private Image image3;
    
    [SerializeField] private GameObject upgradeHandler;
    private UpgradeHandling _upgradeObject;

    private UpgradeBaseSO _upgrade1;
    private UpgradeBaseSO _upgrade2;
    private UpgradeBaseSO _upgrade3;
    
    private void Awake()
    {
        _upgradeObject = upgradeHandler.GetComponent<UpgradeHandling>();
    }

    public void PopulateMenu()
    {
        int index = Random.Range(0, _upgradeObject.FullupgradeList.Count - 1);
        descriptionText1.text = _upgradeObject.FullupgradeList[index].upgradeName + ": " +
                                _upgradeObject.FullupgradeList[index].description;
        image1 = _upgradeObject.FullupgradeList[index].image;
        _upgrade1 = _upgradeObject.FullupgradeList[index];
        
        index = Random.Range(0, _upgradeObject.FullupgradeList.Count - 1);
        descriptionText2.text = _upgradeObject.FullupgradeList[index].upgradeName + ": " +
                                _upgradeObject.FullupgradeList[index].description;
        image2 = _upgradeObject.FullupgradeList[index].image;
        _upgrade2 = _upgradeObject.FullupgradeList[index];
        
        index = Random.Range(0, _upgradeObject.FullupgradeList.Count - 1);
        descriptionText3.text = _upgradeObject.FullupgradeList[index].upgradeName + ": " +
                                _upgradeObject.FullupgradeList[index].description;
        image3 = _upgradeObject.FullupgradeList[index].image;
        _upgrade3 = _upgradeObject.FullupgradeList[index];

        upgradeMenu.enabled = true;
        Time.timeScale = 0;
    }

    public void ApplyUpgrade(int buttonNumber)
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

        upgradeMenu.enabled = false;
        Time.timeScale = 1;
    }
}