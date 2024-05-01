using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using Schemes.Data.LogicData.Composition;
using Schemes.Device;
using Schemes.Device.Movement;
using Schemes.Device.Ports;
using Schemes.Device.Wire;
using Schemes.LogicUnit;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Schemes.Dashboard
{
    public class SchemeEditor : MonoBehaviour
    {
        #region CONSTS

        public const string RELAY_KEY = "d3336114-5910-42bd-b5e2-79518d3ff893";
        public const string ONE_BIT_USER_INPUT_KEY = "2b738a45-ac8c-4508-8385-84491fe01b62";
        public const string ONE_BIT_OUTPUT_KEY = "6c6dabff-4878-4697-8001-9ec948eed7cd";
        public const string CONSTANT_VOLTAGE_KEY = "ee5cf222-b539-4e0c-a5b6-179a9561825d";
        
        #endregion

        #region SERIALIZED_FIELDS

        [SerializeField] private SchemeDevice schemeDeviceRef;
        [SerializeField] private SchemeDeviceWire schemeDeviceWireRef;

        #endregion

        #region PRIVATE_FIELDS

        [DisableInPlayMode] [DisableInEditorMode] [ShowInInspector]
        private Scheme _currentScheme;
        
        private SchemeLogicUnit _currentSchemeLogicUnit;
        private CompositionLogicData _currentCompositionLogicData;
        private SchemeDeviceWire _currentWire;
        private IGridHandler _gridHandler;
        private int _incrementComponentIndex;
        private bool _pendingForWireConnection = false;
        #endregion


        #region GETTERS

        [Obsolete("Consider getting rid of this. This is debug only")]
        public SchemeLogicUnit CurrentSchemeLogicUnit_Debug => _currentSchemeLogicUnit;
        
        [Obsolete("Consider getting rid of this. This is debug only")]
        public Scheme CurrentScheme_Debug => _currentScheme;
        
        #endregion
        
        public void Init()
        {
            _currentScheme = new Scheme();
            _currentCompositionLogicData = _currentScheme.SchemeData.SchemeLogicData as CompositionLogicData;
            _currentSchemeLogicUnit = _currentScheme.InstantiateLogicUnit(-1);
        
            _gridHandler = EditorDashboard.Instance;
        }

        // hack: shortcut for SchemeDevice generation
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                AddDeviceInComposition(InstantiateSchemeDevice(RELAY_KEY));
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                AddDeviceInComposition(InstantiateSchemeDevice(ONE_BIT_USER_INPUT_KEY));
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                AddDeviceInComposition(InstantiateSchemeDevice(ONE_BIT_OUTPUT_KEY));
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                AddDeviceInComposition(InstantiateSchemeDevice(CONSTANT_VOLTAGE_KEY));
            }

            _currentSchemeLogicUnit.Process();
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            // }

            if (Input.GetKeyDown(KeyCode.U))
            {
                SaveScheme();
            }
        
        }

        private void SaveScheme()
        {
            throw new NotImplementedException();
        }


        // ReSharper disable Unity.PerformanceAnalysis
        private SchemeDevice InstantiateSchemeDevice(string schemeKey)
        {
            var schemeDevice = Instantiate(schemeDeviceRef);
            schemeDevice.transform.position = _gridHandler.GetMousePositionToGrid();
            var scheme = GameManager.Instance.GetContainerOfType<SchemesContainer>().GetSchemeByKey(schemeKey);
            schemeDevice.Init(scheme, _incrementComponentIndex);
            schemeDevice.OnDevicePortInteracted += OnDevicePortInteractedHandler;

            // adding movement logic
            var deviceDragDropMovementStrategy = schemeDevice.AddComponent<DragDropMovementStrategy>();
            var gridSnapMovementExecutionStrategy = new GridSnapMovementExecutionStrategy();
            gridSnapMovementExecutionStrategy.SetGirdHandler(EditorDashboard.Instance);

            IMovementExecutionStrategy movementExecutionStrategy = gridSnapMovementExecutionStrategy;
            deviceDragDropMovementStrategy.SetMovementExecutionStrategy(movementExecutionStrategy);
            deviceDragDropMovementStrategy.SetListOfAcceptableColliders(schemeDevice.InteractionColliders);
            deviceDragDropMovementStrategy.ShouldTakeIntoAccountInitialDelta = true;
            deviceDragDropMovementStrategy.EnableMovement();
            _currentSchemeLogicUnit.AddComponentLogicUnit(scheme.InstantiateLogicUnit(_incrementComponentIndex));
        
            return schemeDevice;
        }

        private void RemoveSchemeDevice(SchemeDevice schemeDevice)
        {
            throw new NotImplementedException();
        }

        

        private void OnDevicePortInteractedHandler(SchemeInteractedOnPortsEventArgs arg0)
        {
            if (!_pendingForWireConnection)
            {
                _currentWire = Instantiate(schemeDeviceWireRef);
                // _currentWire.SetGridHandler(_gridHandler);
                _currentWire.SetPosition(arg0.schemeDevicePortInteractEventArgs.port.transform.position);
                _currentWire.SetStartPort(arg0.schemeDevicePortInteractEventArgs.port);
                _currentWire.StartWiring();
                _currentWire.OnWiringCanceled += OnWiringCanceledHandler;
                _pendingForWireConnection = true;
            }
            else
            {
                if (arg0.schemeDevicePortInteractEventArgs.port.SchemeDevice == _currentWire.StartPort.SchemeDevice) return;
                if(_currentWire.StartPort.GetType() == arg0.schemeDevicePortInteractEventArgs.port.GetType()) return;

                _pendingForWireConnection = false;
                _currentWire.TerminateActiveWiring();
                _currentWire.SetEndPort(arg0.schemeDevicePortInteractEventArgs.port);
                DefineRelation(_currentWire.StartPort, _currentWire.EndPort);
            }
        }

        private void OnWiringCanceledHandler()
        {
            _currentWire = null;
            _pendingForWireConnection = false;
        }
    
        private void AddDeviceInComposition(SchemeDevice schemeDevice)
        {
            ComponentScheme componentScheme = new ComponentScheme(_incrementComponentIndex, schemeDevice.UnderliningScheme.SchemeData.SchemeKey);
            _currentCompositionLogicData.ComponentSchemes.Add(componentScheme);
        
            _incrementComponentIndex++;
        }


        // idk what the hell is this going to do
        private void DefineRelation(SchemeDevicePort a, SchemeDevicePort b)
        {
            SchemeRelation schemeRelation = new();

            var ports =  new List<SchemeDevicePort>() { a, b };
            // right relation node
            SchemeDevicePort outputPort = ports.First(x => x is SchemeDeviceOutputPort);
            SchemeDevicePort inputPort = ports.First(x => x is SchemeDeviceInputPort);
        
            ComponentScheme receiverComponent =
                _currentCompositionLogicData.ComponentSchemes.First(c=>c.ComponentIndex == inputPort.SchemeDevice.DeviceIndex);
        
            schemeRelation.receiverNode = new ComponentRelationNode(receiverComponent.ComponentIndex,  inputPort.PortIndex);
        
            // left relation node
            ComponentScheme senderComponent =
                _currentCompositionLogicData.ComponentSchemes.First(c=>c.ComponentIndex == outputPort.SchemeDevice.DeviceIndex);

        
            schemeRelation.senderNode = new ComponentRelationNode(senderComponent.ComponentIndex,  outputPort.PortIndex);
        
            _currentCompositionLogicData.SchemeRelations.Add(schemeRelation);
        }
    
        private void RemoveRelation(SchemeDevicePort a, SchemeDevicePort b)
        {
            throw new NotImplementedException();
        }
    }
}