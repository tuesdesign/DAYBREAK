using UI.Scripts.MainMenu;
using UI.Scripts.Misc_;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuState : MenuBaseState
{
    private GameObject _menuCanvas;
    private GameObject _mainButton;

    public override void EnterState(MenuStateManager menu)
    {
        _menuCanvas = MainMenuManager.Instance.mainMenu;
        _mainButton = MainMenuManager.Instance.mainMenuPrimary;

        _menuCanvas.SetActive(true);

        if (MenuStateManager.Instance.isMobile)
        {
            MainMenuManager.Instance.mobileTitleObject.SetActive(true);
            MainMenuManager.Instance.pcTitleObject.SetActive(false);
            
            MainMenuManager.Instance.pcButtons.SetActive(false);
            MainMenuManager.Instance.mobileButtons.SetActive(true);
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
        _menuCanvas.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
}