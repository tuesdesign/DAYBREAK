using UI.Scripts.MainMenu;
using UI.Scripts.Misc_;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainSettingsState : MenuBaseState
{
    private GameObject _menuCanvas;
    private GameObject _mainButton;
    
    public override void EnterState(MenuStateManager menu)
    {
        _menuCanvas = MainMenuManager.Instance.settingsMenu;
        _mainButton = MainMenuManager.Instance.settingsMenuPrimary;
        
        _menuCanvas.SetActive(true);
        LeanTween.scale(_menuCanvas.gameObject.transform.GetChild(0).gameObject, Vector3.one, 0.2f).setIgnoreTimeScale(true);
        
        if (MenuStateManager.Instance.isMobile)
        {
            MainMenuManager.Instance.mouseAimButton.SetActive(false);
            MainMenuManager.Instance.notifButton.SetActive(true);
        }
        else
        {
            MainMenuManager.Instance.mouseAimButton.SetActive(true);
            MainMenuManager.Instance.notifButton.SetActive(false);
        }
        
        UpdateState(MenuStateManager.Instance);
    }
    
    public override void UpdateState(MenuStateManager menu)
    {
        if (!MenuStateManager.Instance.isMobile)
            EventSystem.current.SetSelectedGameObject(ControllerCheck.Instance.controllerConnected ? _mainButton : null);
    }

    public override void ExitState(MenuStateManager menu)
    {
        LeanTween.scale(_menuCanvas.gameObject.transform.GetChild(0).gameObject, Vector3.zero, 0.2f).setOnComplete(Close).setIgnoreTimeScale(true);
    }

    private void Close()
    {
        _menuCanvas.SetActive(false);
    }
}
