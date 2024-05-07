using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Schemes.Device.Ports
{
    public abstract class SchemeDevicePort : SerializedMonoBehaviour
    {
        [OdinSerialize] private PortValueIndicator portValueIndicator;
        [OdinSerialize] private ISchemeDevicePortInteractionHandler _schemeDevicePortInteractionHandler;

        [DisableInPlayMode] [DisableInEditorMode] [ShowInInspector]
        protected byte portIndex;

        private SchemeDevice _schemeDeviceBelonged;
        public event UnityAction<SchemeDevicePortInteractEventArgs> OnPortClicked;

        public SchemeDevice SchemeDevice => _schemeDeviceBelonged;
        public byte PortIndex => portIndex;
        public void Awake()
        {
            _schemeDevicePortInteractionHandler.OnPortClicked += OnPortClickedHandler;
        }

        public virtual void Init(SchemeDevice schemeDeviceBelonged, byte portIndex)
        {
            this.portIndex = portIndex;
            _schemeDeviceBelonged = schemeDeviceBelonged;
            portValueIndicator.Init(_schemeDeviceBelonged.DeviceIndex, portIndex);
        }

        public virtual void OnPortClickedHandler(SchemeDevicePortInteractEventArgs arg0)
        {
            OnPortClicked?.Invoke(arg0);
        }
        
        public void UpdatePortValue(bool portValue)
        {
            portValueIndicator.UpdatePortValue(portValue);
        }
        
    }

    public interface ISchemeDevicePortInteractionHandler : IPointerClickHandler
    {
        event UnityAction<SchemeDevicePortInteractEventArgs> OnPortClicked;
    }

    public class SchemeDevicePortInteractEventArgs : EventArgs
    {
        public ISchemeDevicePortInteractionHandler schemeDevicePortInteractionHandler;
        public Transform interactedTransform;
        public SchemeDevicePort port;

        public SchemeDevicePortInteractEventArgs(ISchemeDevicePortInteractionHandler schemeDevicePortInteractionHandler,
            Transform interactedTransform, SchemeDevicePort port)
        {
            this.schemeDevicePortInteractionHandler = schemeDevicePortInteractionHandler;
            this.interactedTransform = interactedTransform;
            this.port = port;
        }
    }
}