using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Misc;
using Schemes.Dashboard;
using Schemes.Data;
using Schemes.Data.LogicData;
using Schemes.Data.LogicData.UserIO;
using Schemes.Device.Ports;
using Schemes.Device.Wire;
using Schemes.LogicUnit;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

            if (scheme.SchemeData.SchemeLogicData is UserInputLogicData userInputLogicData)
            {
                gameObject.AddComponent<User1BitInputInteractionHandler>().Init(userInputLogicData, deviceIndex);
            }

            if (scheme.SchemeData.SchemeLogicData is UserOutputLogicData userOutputLogicData)
            {
                gameObject.AddComponent<User1BitOutputIndicator>().Init(userOutputLogicData, deviceIndex);
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
    
    [RequireComponent(typeof(SchemeDevice))]
    public class User1BitInputInteractionHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private UserInputLogicData _userInputLogicData;
        
        private bool _value;
        private int _deviceIndex;
        private TextMeshPro _valueFromUserTextIndicator;
        // SchemeLogicUnit SchemeLogicUnit
        
        public void Init(UserInputLogicData userInputLogicData, int deviceIndex)
        {
            _deviceIndex = deviceIndex;
            _userInputLogicData = userInputLogicData;
            _value = false;
            _valueFromUserTextIndicator = Utilities.CreateWorldText(_value.ToString(), transform, new Vector3(0f, 0f, 1f));
            _valueFromUserTextIndicator.transform.localEulerAngles = new Vector3(90, 0f, 0f);
            _valueFromUserTextIndicator.fontSize = 10;
        }
        
        public void ToggleState()
        {
            _value = !_value;
            GetLogicUnit().Outputs[0] = new LogicUnitPort { Value = _value, IsDefined =  true};
            _valueFromUserTextIndicator.text = _value.ToString();
        }
        
        private SchemeLogicUnit GetLogicUnit()
        {
            return EditorDashboard.Instance.SchemeEditor_Debug.CurrentSchemeLogicUnit_Debug.ComponentLogicUnits.First(
                x => x.index == _deviceIndex);
        }


        private bool _pendingForValueSet;
        private Vector3 _positionOnPointerDown;
        public void OnPointerDown(PointerEventData eventData)
        {
            _pendingForValueSet = true;
            _positionOnPointerDown = transform.position;
        }
        
        // տապոռ way of doing things...
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pendingForValueSet)
            {
                if (transform.position == _positionOnPointerDown)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        ToggleState();
                    }
                }
            }
            _pendingForValueSet = false;
            
        }
    }
    
    [RequireComponent(typeof(SchemeDevice))]
    public class User1BitOutputIndicator : MonoBehaviour
    {
        private UserOutputLogicData _userOutputLogicData;
        private TextMeshPro _valueFromUserTextIndicator;

        private int _deviceIndex;
        private bool _value;
        public void Init(UserOutputLogicData userOutputLogicData, int deviceIndex)
        {
            _userOutputLogicData = userOutputLogicData;
            _deviceIndex = deviceIndex;
            _value = false;
            _valueFromUserTextIndicator = Utilities.CreateWorldText(_value.ToString(), transform, new Vector3(0f, 0f, 1f));
            _valueFromUserTextIndicator.transform.localEulerAngles = new Vector3(90, 0f, 0f);
            _valueFromUserTextIndicator.fontSize = 10;
        }

        private void Update()
        {
            _value = GetLogicUnit().Inputs[0].Value;
            _valueFromUserTextIndicator.text = _value.ToString();
        }

        private SchemeLogicUnit GetLogicUnit()
        {
            return EditorDashboard.Instance.SchemeEditor_Debug.CurrentSchemeLogicUnit_Debug.ComponentLogicUnits.First(
                x => x.index == _deviceIndex);
        }
    }
}