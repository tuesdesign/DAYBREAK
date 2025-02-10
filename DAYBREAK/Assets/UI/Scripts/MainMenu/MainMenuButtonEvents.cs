using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Scripts.MainMenu
{
    public class MainMenuButtonEvents : MonoBehaviour, IPointerExitHandler
    {
        public void OnPointerExit(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}