using Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Scripts.MainMenu
{
    public class MainMenuButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [SerializeField] private bool isCharacterButton;
        private MainMenuAnimator _animator;

        private void Start()
        {
            _animator = FindObjectOfType(typeof(MainMenuAnimator)) as MainMenuAnimator;
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

            SoundFXManager.Instance.PlaySoundFXClip(
                isCharacterButton
                    ? AudioClipManager.Instance.largeSelectSoundClip
                    : AudioClipManager.Instance.smallSelectSoundClip, transform, 1f);
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

            SoundFXManager.Instance.PlaySoundFXClip(
                isCharacterButton
                    ? AudioClipManager.Instance.largeSelectSoundClip
                    : AudioClipManager.Instance.smallSelectSoundClip, transform, 1f);
        }
    }
}