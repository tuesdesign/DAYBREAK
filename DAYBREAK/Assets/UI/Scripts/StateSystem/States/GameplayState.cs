using UI.Scripts;
using UnityEngine;

public class GameplayState : MenuBaseState
{
    private GameObject _menuCanvas;
    
    public override void EnterState(MenuStateManager menu)
    {
        _menuCanvas = UIManager.Instance.gameplayUI;
        
        _menuCanvas.SetActive(true);
    }

    public override void UpdateState(MenuStateManager menu) { }

    public override void ExitState(MenuStateManager menu)
    {
        _menuCanvas.SetActive(false);
    }
}
