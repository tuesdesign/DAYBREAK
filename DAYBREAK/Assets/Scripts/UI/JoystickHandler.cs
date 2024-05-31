using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;

public class JoystickHandler : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image outerCircle;//Outer boundary of joystick.
    private float bgImageSizeX, bgImageSizey;
    private Image innerCircle;//Inner circle of joystick.

    float offsetFactorWithBgSize = 0.5f;
    public static event Action<Vector2> onJoyStickMoved;

    public bool isTouched = false;

    public Vector2 InputDirection { set; get; }


    // Start is called before the first frame update
    void Start()
    {

        outerCircle = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        innerCircle = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        bgImageSizeX = outerCircle.rectTransform.sizeDelta.x;
        bgImageSizey = outerCircle.rectTransform.sizeDelta.y;
    }

    public void OnDrag(PointerEventData ped)
    {
        Vector2 tappedpOint;
        //This if statment gives local position of the pointer at "out touchPoint"
        //if we press or touched inside the area of outerCircle
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle
            (outerCircle.rectTransform, ped.position, ped.pressEventCamera, out tappedpOint))
        {

            //Getting tappedPoint position in fraction where  maxmimum value would be in denominator of below fraction.
            tappedpOint.x = (tappedpOint.x / (bgImageSizeX * offsetFactorWithBgSize));
            tappedpOint.y = (tappedpOint.y / (bgImageSizey * offsetFactorWithBgSize));

            InputDirection = new Vector3(tappedpOint.x, tappedpOint.y);
            InputDirection = InputDirection.magnitude > 1 ? InputDirection.normalized : InputDirection;
            //Updating position of inner circle of joystick.
            innerCircle.rectTransform.anchoredPosition =
                new Vector3(InputDirection.x * (outerCircle.rectTransform.sizeDelta.x * offsetFactorWithBgSize),
                    InputDirection.y * (outerCircle.rectTransform.sizeDelta.y * offsetFactorWithBgSize));

            onJoyStickMoved?.Invoke(InputDirection);

        }
    }
    public GameObject joyStickparent;
    public virtual void OnPointerDown(PointerEventData ped)
    {

        Vector2 initMousePos = new Vector3(ped.position.x, ped.position.y, ped.pressEventCamera.nearClipPlane);
        //joyStickparent.SetActive(true);
        joyStickparent.transform.position = initMousePos;
        isTouched = true;
        OnDrag(ped);

    }

    /// <summary>
    /// Unity function called when mouse button is not pressed or no touch detected in touch screen device.
    ///Disabling joystick and resetting joystick innerCircle to zero position.
    /// </summary>
    public virtual void OnPointerUp(PointerEventData ped)
    {

        InputDirection = Vector2.zero;
        innerCircle.rectTransform.anchoredPosition = Vector3.zero;
        //joyStickparent.gameObject.SetActive(false);
        isTouched = false;
        onJoyStickMoved?.Invoke(InputDirection);
        
    }

}