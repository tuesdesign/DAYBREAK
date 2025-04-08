using Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Scripts.PauseMenu
{
    public class PauseMenuButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        private PauseMenuAnimator _animator;

        private void Start()
        {
            _animator = FindObjectOfType(typeof(PauseMenuAnimator)) as PauseMenuAnimator;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _animator.ButtonHover(gameObject);
            
            SoundFXManager.Instance.PlaySoundFXClip(AudioClipManager.Instance.hoverSoundClip, transform, 1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _animator.ButtonExit(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _animator.ButtonClick(gameObject);
            
            SoundFXManager.Instance.PlaySoundFXClip(AudioClipManager.Instance.smallSelectSoundClip, transform, 1f);
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            _animator.ButtonHover(gameObject);
            
            SoundFXManager.Instance.PlaySoundFXClip(AudioClipManager.Instance.hoverSoundClip, transform, 1f);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _animator.ButtonExit(gameObject);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            _animator.ButtonClick(gameObject);
            
            SoundFXManager.Instance.PlaySoundFXClip(AudioClipManager.Instance.smallSelectSoundClip, transform, 1f);
        }
    }
}
