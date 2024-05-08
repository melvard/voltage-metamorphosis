using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Schemes.Device.Ports
{
    [RequireComponent(typeof(SchemeDevicePort))]
    public class SchemeDevicePortInteractionHandler : MonoBehaviour, ISchemeDevicePortInteractionHandler
    {
        private SchemeDevicePort _schemeDevicePort;
        public event UnityAction<SchemeDevicePortInteractEventArgs> OnPortClicked;

        // or start
        private void Awake()
        {
            _schemeDevicePort = GetComponent<SchemeDevicePort>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                OnPortClicked?.Invoke(new SchemeDevicePortInteractEventArgs(this, transform, _schemeDevicePort));
            }
        }
    }
}