using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Scripts.PauseMenu
{
    public class PauseMenuButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            PauseMenuAnimator.Instance.ButtonHover(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PauseMenuAnimator.Instance.ButtonExit(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PauseMenuAnimator.Instance.ButtonClick(gameObject);
        }
    }
}
