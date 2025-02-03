using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Scripts.MainMenu
{
    public class MainMenuButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        private MainMenuAnimator _animator;

        private void Start()
        {
            _animator = FindObjectOfType(typeof(MainMenuAnimator)) as MainMenuAnimator;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _animator.ButtonHover(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _animator.ButtonExit(gameObject);
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _animator.ButtonClick(gameObject);
        }

        public void OnSelect(BaseEventData eventData)
        {
            _animator.ButtonHover(gameObject);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _animator.ButtonExit(gameObject);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            _animator.ButtonClick(gameObject);
        }
    }
}