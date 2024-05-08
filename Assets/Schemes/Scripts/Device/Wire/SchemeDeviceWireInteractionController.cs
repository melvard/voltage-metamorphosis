using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Schemes.Device.Wire
{
    public class SchemeDeviceWireInteractionController : MonoBehaviour, IPointerClickHandler
    {
        public event UnityAction OnRemoveWireClick;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                OnRemoveWireClick?.Invoke();
                Debug.Log("OnRemoveWireClick");
            }
        }
    }
}