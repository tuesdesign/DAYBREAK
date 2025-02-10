using UnityEngine;

public class MenuStateManager : MonoBehaviour
{
    // Current state
    public MenuBaseState CurrentState;

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


    private bool _forcedExit;
    
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
        if (!_forcedExit)
            CurrentState.ExitState(this);
        
        CurrentState = state;
        state.EnterState(this);

        _forcedExit = false;
    }

    public void ForceExitState()
    {
        _forcedExit = true;
        CurrentState.ExitState(this);
    }
}