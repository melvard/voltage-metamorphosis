using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Misc;
using Schemes.Data.LogicData;
using Schemes.Data.LogicData.UserIO;
using Schemes.Device.Ports;
using Schemes.LogicUnit;
using Sirenix.OdinInspector;
using TMPro;
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

        [ShowInInspector] [DisableInEditorMode]
        private ISchemeDeviceVisualizer _schemeDeviceVisualizer;
        
        #endregion

        #region GETTERS

        public Scheme UnderliningScheme => _underliningScheme;
        public SchemeLogicUnit LogicUnit => _schemeLogicUnit;

        #endregion

        #region EVENTS

        [CanBeNull] public event UnityAction<SchemeInteractedOnPortsEventArgs> OnDevicePortInteracted;

        #endregion

        #region PRIVATE_FIELDS

        private SchemeLogicUnit _schemeLogicUnit;

        #endregion

        public void Init(Scheme scheme)
        {
            _schemeDeviceVisualizer ??= GetComponent<ISchemeDeviceVisualizer>();
            _schemeLogicUnit = scheme.InstantiateLogicUnit();
            
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

            if (scheme.SchemeData.SchemeLogicData is UserInputLogicData userInputLogicData)
            {
                gameObject.AddComponent<User1BitInputInteractionHandler>().Init(userInputLogicData);
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

    public class User1BitInputInteractionHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private UserInputLogicData _userInputLogicData;
        private byte _value;
        private TextMeshPro _valueFromUserTextIndicator;
        
        public void Init(UserInputLogicData userInputLogicData)
        {
            _userInputLogicData = userInputLogicData;
            _value = 0;
            _valueFromUserTextIndicator = Utilities.CreateWorldText(_value.ToString(), transform, new Vector3(0f, 0f, 1f));
            _valueFromUserTextIndicator.transform.localEulerAngles = new Vector3(90, 0f, 0f);
            // _valueFromUserTextIndicator.alignment = TextAlignmentOptions.Center;
            // _valueFromUserTextIndicator.anchor = TextAnchor.MiddleCenter;
            _valueFromUserTextIndicator.fontSize = 10;
        }
        
        public void ToggleState()
        {
            _value = (byte)(_value == 0 ? 1 : 0);
            _valueFromUserTextIndicator.text = _value.ToString();
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
}