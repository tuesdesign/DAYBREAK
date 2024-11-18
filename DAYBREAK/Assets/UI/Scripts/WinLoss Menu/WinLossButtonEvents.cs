using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Scripts.WinLoss_Menu
{
    public class WinLossButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            WinLossAnimator.Instance.ButtonHover(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            WinLossAnimator.Instance.ButtonExit(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            WinLossAnimator.Instance.ButtonClick(gameObject);
        }
    }
}
