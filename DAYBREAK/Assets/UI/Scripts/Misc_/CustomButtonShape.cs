using UnityEngine;
using UnityEngine.UI;

namespace UI.Scripts.Misc_
{
    public class CustomButtonShape : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
        }
    }
}
