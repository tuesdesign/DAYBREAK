using TMPro;
using UI.Scripts.Misc_;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoteSystem : MonoBehaviour
{
    [Header("Notes Displays")]
    [SerializeField] private GameObject notesList;
    [SerializeField] private GameObject noteInfo;
    
    [Header("Text Field")]
    [SerializeField] private TMP_Text notesText;
    
    [Header("Misc")]
    [SerializeField] private GameObject firstNoteButton;
    [SerializeField] private GameObject backButton;

    public void OpenNotePage()
    {
        MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.NotesState);
    }
    
    public void CloseNotePage()
    {
        MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.MainMenuState);
    }

    public void NoteBackButton()
    {
        if (noteInfo.activeSelf)
            CloseNoteText();
        else if (notesList.activeSelf)
            CloseNotePage();
    }
    
    // Open/Close Note Text UI //
        
    public void OpenNoteText(LorePieceSO lorePiece)
    {
        notesList.SetActive(false);
        noteInfo.SetActive(true);
        
        if (!MenuStateManager.Instance.isMobile)
            EventSystem.current.SetSelectedGameObject(ControllerCheck.Instance.controllerConnected ? backButton : null);
        
        notesText.text = lorePiece.loreInformation;
    }

    public void CloseNoteText()
    {
        notesList.SetActive(true);
        noteInfo.SetActive(false);
        
        if (!MenuStateManager.Instance.isMobile)
                    EventSystem.current.SetSelectedGameObject(ControllerCheck.Instance.controllerConnected ? firstNoteButton : null);
    }
}