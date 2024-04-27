using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Schemes.Data;
using Schemes.Device.Ports;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Schemes.Device
{
    [RequireComponent(typeof(ISchemeDeviceVisualizer))]
    public class SchemeDevice : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [AssetsOnly][SerializeField] private SchemeDeviceInputPort schemeDeviceInputPortRef;
        [AssetsOnly][SerializeField] private SchemeDeviceOutputPort schemeDeviceOutputPortRef;
        [field : SerializeField] public List<Collider> InteractionColliders { get; private set; }

        [Title("Info")] [ShowInInspector] [DisableInEditorMode]
        private Scheme _underliningScheme;

        [ShowInInspector] [DisableInEditorMode]
        private ISchemeDeviceVisualizer _schemeDeviceVisualizer;

        #endregion

        #region GETTERS

        public Scheme UnderliningScheme => _underliningScheme;

        #endregion

        #region EVENTS

        [CanBeNull] public event UnityAction<SchemeInteractedOnPortsEventArgs> OnDevicePortInteracted;

        #endregion

        public void Init(Scheme scheme)
        {
            _schemeDeviceVisualizer ??= GetComponent<ISchemeDeviceVisualizer>();

            _underliningScheme = scheme;
            List<SchemeDeviceInputPort> schemeDeviceInputPorts = null;
            List<SchemeDeviceOutputPort> schemeDeviceOutputPorts = null;
            if (scheme.SchemeData.SchemeLogicData is IInputPortSchemesLogicData inputPortSchemesLogicData)
            {
                schemeDeviceInputPorts = GenerateInputPorts(inputPortSchemesLogicData.NumberOfInputs);
            }

            if (scheme.SchemeData.SchemeLogicData is IOutputPortSchemeLogicData outputPortSchemeLogicData)
            {
                schemeDeviceOutputPorts = GenerateOutputPorts(outputPortSchemeLogicData.NumberOfOutputs);
            }


            SchemeDeviceVisualsData schemeDeviceVisualsData = new()
            {
                schemeVisualsData = scheme.SchemeData.SchemeVisualsData,
                schemeDeviceInputPorts = schemeDeviceInputPorts,
                schemeDeviceOutputPorts = schemeDeviceOutputPorts
            };
            _schemeDeviceVisualizer.Visualise(schemeDeviceVisualsData);

            // _schemeLogicUnit.Logigalize(scheme.SchemeData.SchemeLogicData);
        }

        private List<SchemeDeviceInputPort> GenerateInputPorts(byte amountOfInputs)
        {
            List<SchemeDeviceInputPort> schemeDeviceInputPorts = new();
            for (byte i = 0; i < amountOfInputs; i++)
            {
                var schemeDeviceInputPort = Instantiate(schemeDeviceInputPortRef, transform);
                schemeDeviceInputPort.Init(i);
                schemeDeviceInputPort.OnPortClicked += OnPortClickedHandler;
                schemeDeviceInputPorts.Add(schemeDeviceInputPort);
            }

            return schemeDeviceInputPorts;
        }

        private void OnPortClickedHandler(SchemeDevicePortInteractEventArgs arg0)
        {
            OnDevicePortInteracted?.Invoke(new SchemeInteractedOnPortsEventArgs(this, arg0));
        }

        private List<SchemeDeviceOutputPort> GenerateOutputPorts(byte amountOfInputs)
        {
            List<SchemeDeviceOutputPort> schemeDeviceOutputPorts = new();
            for (byte i = 0; i < amountOfInputs; i++)
            {
                var schemeDeviceOutputPort = Instantiate(schemeDeviceOutputPortRef, transform);
                schemeDeviceOutputPort.Init(i);
                schemeDeviceOutputPort.OnPortClicked += OnPortClickedHandler;
                schemeDeviceOutputPorts.Add(schemeDeviceOutputPort);
            }

            return schemeDeviceOutputPorts;
        }
    }

    public class SchemeInteractedOnPortsEventArgs : EventArgs
    {
        public SchemeDevicePortInteractEventArgs schemeDevicePortInteractEventArgs;
        public SchemeDevice schemeDevice;

        public SchemeInteractedOnPortsEventArgs(SchemeDevice schemeDevice,
            SchemeDevicePortInteractEventArgs schemeDevicePortInteractEventArgs)
        {
            this.schemeDevice = schemeDevice;
            this.schemeDevicePortInteractEventArgs = schemeDevicePortInteractEventArgs;
        }
    }
}