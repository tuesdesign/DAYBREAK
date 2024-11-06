using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Scripts.MainMenu
{
    public class MainMenuButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            MainMenuAnimator.Instance.ButtonHover(this.gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MainMenuAnimator.Instance.ButtonExit(this.gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MainMenuAnimator.Instance.ButtonClick(this.gameObject);
        }
    }
}