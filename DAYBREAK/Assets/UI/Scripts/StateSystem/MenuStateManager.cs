using UnityEngine;
using UnityEngine.EventSystems;

public class MenuStateManager : MonoBehaviour
{
    // Current state
    public MenuBaseState CurrentState;
    public bool forcedExit;
    
    public bool isMobile;
    
    
    // All possible states
    public AchievementsState AchievementsState = new AchievementsState();
    public CharacterSelectState CharacterSelectState = new CharacterSelectState();
    public GameplaySettingsState GameplaySettingsState = new GameplaySettingsState();
    public GameplayState GameplayState = new GameplayState();
    public MainMenuState MainMenuState = new MainMenuState();
    public MainSettingsState MainSettingsState = new MainSettingsState();
    public NotesState NotesState = new NotesState();
    public PauseState PauseState = new PauseState();
    public UpgradeState UpgradeState = new UpgradeState();
    public WinLossState WinLossState = new WinLossState();
    
    
    public static MenuStateManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        // Starting state for the state machine
        CurrentState = MainMenuState;
        
        CurrentState.EnterState(this);
    }

    public void UpdateState()
    {
        CurrentState.UpdateState(this);
    }

    public void SetMenuState(MenuBaseState state)
    {
        if (!forcedExit)
            CurrentState.ExitState(this);
        
        CurrentState = state;
        state.EnterState(this);

        forcedExit = false;
    }

    public void ForceExitState()
    {
        forcedExit = true;
        CurrentState.ExitState(this);
    }
}