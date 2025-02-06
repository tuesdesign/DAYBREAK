using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Scripts.Misc_
{
    [RequireComponent(typeof(ScrollRect))]
    public class AutoScrollRect : MonoBehaviour
    {
        public float scrollSpeed = 10f;
        public ScrollRect mTemplateScrollRect;
        public RectTransform mTemplateViewportTransform;
        public RectTransform mContentRectTransform;

        private RectTransform _mSelectedRectTransform;
        
        public bool menuOpen;
        public bool notesMenuOpen;
        
        private ControllerCheck _controllerCheck;

        private void Start()
        {
            _controllerCheck = FindObjectOfType(typeof(ControllerCheck)) as ControllerCheck;
        }
        
        void Update()
        {
            if (_controllerCheck.connected && (menuOpen || notesMenuOpen))
                UpdateScrollToSelected(mTemplateScrollRect, mContentRectTransform, mTemplateViewportTransform);
        }

        void UpdateScrollToSelected(ScrollRect scrollRect, RectTransform contentRectTransform, RectTransform viewportRectTransform)
        {
            // Get the current selected option from the eventsystem.
            var selected = EventSystem.current.currentSelectedGameObject;

            if (selected == null)
                return;
            
            if (selected.transform.parent != contentRectTransform.transform)
                return;

            _mSelectedRectTransform = selected.GetComponent<RectTransform>();

            // Math stuff
            var selectedDifference = viewportRectTransform.localPosition - _mSelectedRectTransform.localPosition;
            var contentHeightDifference = (contentRectTransform.rect.height - viewportRectTransform.rect.height);

            var selectedPosition = (contentRectTransform.rect.height - selectedDifference.y);
            var currentScrollRectPosition = scrollRect.normalizedPosition.y * contentHeightDifference;
            var above = currentScrollRectPosition - (_mSelectedRectTransform.rect.height / 2) + viewportRectTransform.rect.height;
            var below = currentScrollRectPosition + (_mSelectedRectTransform.rect.height / 2);

            // Check if selected option is out of bounds.
            if (selectedPosition > above)
            {
                var step = selectedPosition - above;
                var newY = currentScrollRectPosition + step;
                var newNormalizedY = newY / contentHeightDifference;
                scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, new Vector2(0, newNormalizedY), scrollSpeed * Time.deltaTime);
            }
            else if (selectedPosition < below)
            {
                var step = selectedPosition - below;
                var newY = currentScrollRectPosition + step;
                var newNormalizedY = newY / contentHeightDifference;
                scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, new Vector2(0, newNormalizedY), scrollSpeed * Time.deltaTime);
            }
        }
    }
}