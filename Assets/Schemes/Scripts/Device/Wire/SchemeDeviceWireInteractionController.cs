using UnityEngine;
using UnityEngine.EventSystems;

namespace Schemes.Device.Wire
{
    public class SchemeDeviceWireInteractionController : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click event works on wire.");
        }
    }
}