using UI.Scripts.MainMenu;
using UI.Scripts.Misc_;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelectState : MenuBaseState
{
    private GameObject _menuCanvas;
    private GameObject _mainButton;
    
    public override void EnterState(MenuStateManager menu)
    {
        _menuCanvas = MainMenuManager.Instance.characterSelect;
        _mainButton = MainMenuManager.Instance.characterSelectPrimary;
        
        _menuCanvas.SetActive(true);
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
