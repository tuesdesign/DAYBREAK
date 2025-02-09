using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Scripts.Misc_;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI.Scripts.Notes
{
    public class NotesManager : MonoBehaviour
    {
        [SerializeField] private NewDict noteDict;
        [SerializeField] public Dictionary<int, string> notes;

        [Header("Notes List")]
        [SerializeField] private GameObject notesScrollList;
        [SerializeField] private AutoScrollRect notesAutoScrollRect;
        
        [Header("Text Field")]
        [SerializeField] private GameObject notesTextBackground;
        [SerializeField] private TMP_Text notesText;

        [Header("Misc.")]
        [SerializeField] private GameObject firstNoteButton;
        [SerializeField] private GameObject backButton;
        [SerializeField] private GameObject noteButton;
        [SerializeField] private GameObject mainButtonUI;
        [SerializeField] private GameObject quitButton;
        
        
        private bool _notesOpen;
        
        private ControllerCheck _controllerCheck;
        
        private void Start()
        {
            notes = noteDict.ToDictionary();
            _controllerCheck = FindObjectOfType(typeof(ControllerCheck)) as ControllerCheck;
        }
        
        // Open/Close Note List //
        
        public void ToggleNotesList()
        {
            _notesOpen = !_notesOpen;

            if (_notesOpen)
            {
                notesScrollList.SetActive(true);
                LeanTween.scaleY(notesScrollList, 1, 0.3f).setIgnoreTimeScale(true);
                notesAutoScrollRect.notesMenuOpen = true;

                var nav = noteButton.GetComponent<Button>().navigation;
                nav.selectOnUp = firstNoteButton.GetComponent<Button>();
                noteButton.GetComponent<Button>().navigation = nav;
            }
            else
            {
                StartCoroutine(NotesClose());
                notesAutoScrollRect.notesMenuOpen = false;
                
                var nav = noteButton.GetComponent<Button>().navigation;
                nav.selectOnUp = null;
                noteButton.GetComponent<Button>().navigation = nav;
            }
        }
        
        private IEnumerator NotesClose()
        {
            LeanTween.scaleY(notesScrollList, 0, 0.3f).setIgnoreTimeScale(true);

            yield return new WaitForSecondsRealtime(0.3f);
            
            notesScrollList.SetActive(true);
        }

        // Open/Close Note Text UI //
        
        public void OpenNoteText(int noteNum)
        {
            notesTextBackground.SetActive(true);
            notesText.text = notes[noteNum];
            
            // Change button Nav.
            _controllerCheck.SetSelectedButton(backButton);
            quitButton.GetComponent<Button>().interactable = false;
            mainButtonUI.SetActive(false); 
        }

        public void CloseNoteText()
        {
            notesTextBackground.SetActive(false);
            
            // Change button Nav.
            _controllerCheck.SetSelectedButton(noteButton);
            quitButton.GetComponent<Button>().interactable = true;
            mainButtonUI.SetActive(true); 
        }
    }
    
    // Serializable Dict stuff //
    
    [Serializable]
    public class NewDict
    {
        [SerializeField] private NewDictItem[] thisDictItem;

        public Dictionary<int, string> ToDictionary()
        {
            Dictionary<int, string> newDict = new Dictionary<int, string>();

            foreach (var item in thisDictItem)
            {
                newDict.Add(item.noteNum, item.noteText);
            }

            return newDict;
        }
    }

    [Serializable]
    public class NewDictItem
    {
        [SerializeField] public int noteNum;
        [SerializeField] public string noteText;
    }
}
