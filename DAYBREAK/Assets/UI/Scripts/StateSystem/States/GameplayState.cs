using UI.Scripts;
using UI.Scripts.Misc_;
using UnityEngine;

public class GameplayState : MenuBaseState
{
    private GameObject _menuCanvas;
    
    public override void EnterState(MenuStateManager menu)
    {
        _menuCanvas = UIManager.Instance.gameplayUI;
        
        _menuCanvas.SetActive(true);

        if (menu.isMobile)
        {
            UIManager.Instance.touchCanvas.SetActive(true);
        }

        if (PlayerPrefs.GetInt("MouseAim") == 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public override void UpdateState(MenuStateManager menu) { }

    public override void ExitState(MenuStateManager menu)
    {
        _menuCanvas.SetActive(false);
        
        Cursor.visible = ControllerCheck.Instance.controllerConnected != true;
        Cursor.lockState = ControllerCheck.Instance.controllerConnected ? CursorLockMode.Locked : CursorLockMode.None;
        
        if (menu.isMobile)
        {
            //UIManager.Instance.mobileSpecificUI.SetActive(false);
            UIManager.Instance.touchCanvas.SetActive(false);
        }
    }
}
