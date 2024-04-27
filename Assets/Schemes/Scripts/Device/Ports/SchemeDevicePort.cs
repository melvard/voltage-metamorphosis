using System;
using System.Diagnostics;
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
        [OdinSerialize] private ISchemeDevicePortInteractionHandler _schemeDevicePortInteractionHandler;

        [DisableInPlayMode] [DisableInEditorMode] [ShowInInspector]
        protected byte portIndex;

        public event UnityAction<SchemeDevicePortInteractEventArgs> OnPortClicked;

        public void Awake()
        {
            _schemeDevicePortInteractionHandler.OnPortClicked += OnPortClickedHandler;
        }

        public virtual void Init(byte portIndex)
        {
            this.portIndex = portIndex;
        }

        public virtual void OnPortClickedHandler(SchemeDevicePortInteractEventArgs arg0)
        {
            OnPortClicked?.Invoke(arg0);
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