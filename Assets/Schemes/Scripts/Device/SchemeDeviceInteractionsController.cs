using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Schemes.Device
{
    [RequireComponent(typeof(SchemeDevice))]
    public class SchemeDeviceInteractionsController : MonoBehaviour, IPointerClickHandler
    {
        public event UnityAction OnDeviceRemoveClick;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                OnDeviceRemoveClick?.Invoke();
                // Debug.Log("Mouse1 clicked on device ");
            }
        }
    }
}