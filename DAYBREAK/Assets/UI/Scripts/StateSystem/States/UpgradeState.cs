using UI.Scripts;
using UI.Scripts.Misc_;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeState : MenuBaseState
{
    private GameObject _menuCanvas;
    private GameObject _mainButton;
    
    public override void EnterState(MenuStateManager menu)
    {
        _menuCanvas = UIManager.Instance.upgradeMenu;
        _mainButton = UIManager.Instance.upgradeMenuPrimary;
        
        _menuCanvas.SetActive(true);
        ScaleChild0();
        UpdateState(MenuStateManager.Instance);
    }

    public override void UpdateState(MenuStateManager menu)
    {
        if (!MenuStateManager.Instance.isMobile)
            EventSystem.current.SetSelectedGameObject(ControllerCheck.Instance.controllerConnected ? _mainButton : null);
    }

    public override void ExitState(MenuStateManager menu)
    {
        foreach (Transform child in _menuCanvas.gameObject.transform.GetChild(1).gameObject.transform)
        {
            LeanTween.scale(child.gameObject, Vector3.zero, 0.2f).setOnComplete(Close).setIgnoreTimeScale(true);
        }
    }

    private void ScaleChild0()
    {
        LeanTween.scale(_menuCanvas.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject, Vector3.one, 0.2f).setOnComplete(ScaleChild1).setIgnoreTimeScale(true);
    }
    
    private void ScaleChild1()
    {
        LeanTween.scale(_menuCanvas.gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject, Vector3.one, 0.2f).setOnComplete(ScaleChild2).setIgnoreTimeScale(true);
    }
    
    private void ScaleChild2()
    {
        LeanTween.scale(_menuCanvas.gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject, Vector3.one, 0.2f).setIgnoreTimeScale(true);
    }

    private void Close()
    {
        _menuCanvas.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
