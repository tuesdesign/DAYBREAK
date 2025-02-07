using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Scripts.Misc_;
using UnityEngine;

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
        
        private bool _notesOpen;
        
        private void Start()
        {
            notes = noteDict.ToDictionary();
        }
        
        public void ToggleNotesList()
        {
            _notesOpen = !_notesOpen;

            if (_notesOpen)
            {
                notesScrollList.SetActive(true);
                LeanTween.scaleY(notesScrollList, 1, 0.3f).setIgnoreTimeScale(true);
                notesAutoScrollRect.notesMenuOpen = true;
            }
            else
            {
                StartCoroutine(NotesClose());
                notesAutoScrollRect.notesMenuOpen = false;
            }
        }
        
        private IEnumerator NotesClose()
        {
            LeanTween.scaleY(notesScrollList, 0, 0.3f).setIgnoreTimeScale(true);

            yield return new WaitForSecondsRealtime(0.3f);
            
            notesScrollList.SetActive(true);
        }

        public void OpenNoteText(int noteNum)
        {
            notesTextBackground.SetActive(true);
            notesText.text = notes[noteNum];
        }

        public void CloseNoteText()
        {
            notesTextBackground.SetActive(false);
        }
    }
    
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
