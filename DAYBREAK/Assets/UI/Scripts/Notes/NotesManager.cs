using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Scripts.Misc_;
using UnityEngine;
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
        [SerializeField] private TMP_Text notesText;

        [Header("Misc.")]
        [SerializeField] private GameObject firstNoteButton;
        [SerializeField] private GameObject noteButton;
        
        private bool _notesOpen;
        
        private void Start()
        {
            notes = noteDict.ToDictionary();
        }
        
        // Open/Close Note List //
        
        public void ToggleNotesList()
        {
            _notesOpen = !_notesOpen;

            if (_notesOpen)
            {
                notesScrollList.SetActive(true);
                LeanTween.scaleY(notesScrollList, 1, 0.3f).setIgnoreTimeScale(true);

                var nav = noteButton.GetComponent<Button>().navigation;
                nav.selectOnUp = firstNoteButton.GetComponent<Button>();
                noteButton.GetComponent<Button>().navigation = nav;
            }
            else
            {
                StartCoroutine(NotesClose());
                
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
            notesText.text = notes[noteNum];
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.NotesState);
        }

        public void CloseNoteText()
        {
            MenuStateManager.Instance.SetMenuState(MenuStateManager.Instance.MainMenuState);
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
