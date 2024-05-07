using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Schemes.Dashboard;
using Schemes.Data;
using Schemes.Data.LogicData;
using Schemes.Data.LogicData.UserIO;
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

        [AssetsOnly] [SerializeField] private SchemeDeviceInputPort schemeDeviceInputPortRef;
        [AssetsOnly] [SerializeField] private SchemeDeviceOutputPort schemeDeviceOutputPortRef;
        [field: SerializeField] public List<Collider> InteractionColliders { get; private set; }

        [Title("Info")] [ShowInInspector] [DisableInEditorMode]
        private Scheme _underliningScheme;

        #endregion

        #region PRIVATE_FIELDS

        [ShowInInspector] [DisableInEditorMode]
        private ISchemeDeviceVisualizer _schemeDeviceVisualizer;

        private int _deviceIndex;
        private List<SchemeDeviceInputPort> _schemeDeviceInputPorts;
        private List<SchemeDeviceOutputPort> _schemeDeviceOutputPorts;

        #endregion

        #region GETTERS

        public Scheme UnderliningScheme => _underliningScheme;

        public int DeviceIndex => _deviceIndex;
        // public SchemeLogicUnit LogicUnit => _schemeLogicUnit;

        #endregion

        #region EVENTS

        [CanBeNull] public event UnityAction<SchemeInteractedOnPortsEventArgs> OnDevicePortInteracted;

        #endregion

        // #region PRIVATE_FIELDS
        //
        // // private SchemeLogicUnit _schemeLogicUnit;
        //
        // #endregion

        public void Init(Scheme scheme, int deviceIndex)
        {
            _schemeDeviceVisualizer ??= GetComponent<ISchemeDeviceVisualizer>();
            _underliningScheme = scheme;
            _deviceIndex = deviceIndex;

            if (scheme.SchemeData.SchemeLogicData is IInputPortSchemesLogicData inputPortSchemesLogicData)
            {
                _schemeDeviceInputPorts = GenerateInputPorts(inputPortSchemesLogicData.NumberOfInputs);
            }

            if (scheme.SchemeData.SchemeLogicData is IOutputPortSchemeLogicData outputPortSchemeLogicData)
            {
                _schemeDeviceOutputPorts = GenerateOutputPorts(outputPortSchemeLogicData.NumberOfOutputs);
            }

            // Device index is intended to be -1 if device is creating not scheme editing purposes, thus interaction are not handled. Yes, I know that this is not the best
            if (deviceIndex != -1)
            {
                if (scheme.SchemeData.SchemeLogicData is UserInputLogicData userInputLogicData)
                {
                    gameObject.AddComponent<User1BitInputInteractionHandler>().Init(userInputLogicData, deviceIndex);
                }

                if (scheme.SchemeData.SchemeLogicData is UserOutputLogicData userOutputLogicData)
                {
                    gameObject.AddComponent<User1BitOutputIndicator>().Init(userOutputLogicData, deviceIndex);
                }
            }


            SchemeDeviceVisualsData schemeDeviceVisualsData = new()
            {
                schemeVisualsData = scheme.SchemeData.SchemeVisualsData,
                schemeDeviceInputPorts = _schemeDeviceInputPorts,
                schemeDeviceOutputPorts = _schemeDeviceOutputPorts
            };
            _schemeDeviceVisualizer.Visualise(schemeDeviceVisualsData);

            // _schemeLogicUnit.Logigalize(scheme.SchemeData.SchemeLogicData);
        }

        // todo: actively processing schemeLogic unit is not a good option, consider implementing edge triggered function
        private void Update()
        {
            if (_deviceIndex != -1)
            {
                UpdatePortValues();
            }
        }

        private void UpdatePortValues()
        {
            var logicUnit = EditorDashboard.Instance.SchemeEditor_Debug.CurrentSchemeLogicUnit_Debug.ComponentLogicUnits
                .First(
                    x => x.index == _deviceIndex);
            if (_schemeDeviceInputPorts != null)
            {
                foreach (var schemeDeviceInputPort in _schemeDeviceInputPorts)
                {
                    schemeDeviceInputPort.UpdatePortValue(logicUnit.Inputs[schemeDeviceInputPort.PortIndex].Value);
                }
            }

            if (_schemeDeviceOutputPorts != null)
            {
                foreach (var schemeDeviceOutputPort in _schemeDeviceOutputPorts)
                {
                    schemeDeviceOutputPort.UpdatePortValue(logicUnit.Outputs[schemeDeviceOutputPort.PortIndex].Value);
                }
            }

        }

        private List<SchemeDeviceInputPort> GenerateInputPorts(byte amountOfInputs)
        {
            List<SchemeDeviceInputPort> schemeDeviceInputPorts = new();
            for (byte i = 0; i < amountOfInputs; i++)
            {
                var schemeDeviceInputPort = Instantiate(schemeDeviceInputPortRef, transform);
                schemeDeviceInputPort.Init(this, i);
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
                schemeDeviceOutputPort.Init(this, i);
                schemeDeviceOutputPort.OnPortClicked += OnPortClickedHandler;
                schemeDeviceOutputPorts.Add(schemeDeviceOutputPort);
            }

            return schemeDeviceOutputPorts;
        }

        public Coordinate GetCoordinate()
        {
            var dashboardGridElement = EditorDashboard.Instance.GetDashboardElementOnGrid(transform.position);
            return new Coordinate(dashboardGridElement.X, dashboardGridElement.Y);
        }

        public SchemeDeviceInputPort GetInputPortByIndex(int portIndex)
        {
            return _schemeDeviceInputPorts.First(x => x.PortIndex == portIndex);
        }

        public SchemeDeviceOutputPort GetOutputPortByIndex(int portIndex)
        {
            return _schemeDeviceOutputPorts.First(x => x.PortIndex == portIndex);
        }
    }
}