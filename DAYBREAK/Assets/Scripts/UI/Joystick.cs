using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image bgImage;
    private Image joystickImage;
    private Vector2 inputVector;

    float offsetFactorWithBgSize = 0.5f;

    public bool isTouched;

    private void Start()
    {
        bgImage = GetComponent<Image>();
        joystickImage = transform.GetChild(0).GetComponent<Image>();
        
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImage.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / (bgImage.rectTransform.sizeDelta.x * offsetFactorWithBgSize));
            pos.y = (pos.y / (bgImage.rectTransform.sizeDelta.y * offsetFactorWithBgSize));

            inputVector = new Vector2(pos.x, pos.y);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            joystickImage.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImage.rectTransform.sizeDelta.x * offsetFactorWithBgSize), inputVector.y * (bgImage.rectTransform.sizeDelta.y *offsetFactorWithBgSize));
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        
        isTouched = true;

        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        
        isTouched = false;
        inputVector = Vector2.zero;
        joystickImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    public Vector2 GetJoystickVector()
    {
        return inputVector;
    }
    public float Horizontal()
    {
        return inputVector.x;
    }

    public float Vertical()
    {
        return inputVector.y;
    }
}
