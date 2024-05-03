using System;
using System.Collections.Generic;
using System.Linq;
using Canvas;
using Cysharp.Threading.Tasks;
using Exceptions;
using GameLogic;
using Schemes.Data;
using Schemes.Data.LogicData.Composition;
using Schemes.Device;
using Schemes.Device.Movement;
using Schemes.Device.Ports;
using Schemes.Device.Wire;
using Schemes.LogicUnit;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Schemes.Dashboard
{
    public class SchemeEditor : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [AssetsOnly] [SerializeField] private SchemeDevice schemeDeviceRef;
        [AssetsOnly] [SerializeField] private SchemeDeviceWire schemeDeviceWireRef;
        [SerializeField] private SchemeEditorUI schemeEditorUI;
        #endregion

        #region PRIVATE_FIELDS

        [DisableInPlayMode] [DisableInEditorMode] [ShowInInspector]
        private Scheme _currentScheme;

        private SchemeLogicUnit _currentSchemeLogicUnit;
        private CompositionLogicData _currentCompositionLogicData;
        private SchemeDeviceWire _currentWire;

        private List<SchemeDevice> _devices;
        private List<SchemeDeviceWire> _wires;
        private IGridHandler _gridHandler;
        private int _incrementComponentIndex;
        private int _incrementRelationIndex;

        private bool _pendingForWireConnection;

        #endregion

        #region GETTERS

        [Obsolete("Consider getting rid of this. This is debug only")]
        public SchemeLogicUnit CurrentSchemeLogicUnit_Debug => _currentSchemeLogicUnit;

        [Obsolete("Consider getting rid of this. This is debug only")]
        public Scheme CurrentScheme_Debug => _currentScheme;

        #endregion

        #region EVENTS

        public event UnityAction<Scheme> OnLoadedScheme;

        #endregion
        
        public void Init()
        {
            NewScheme();
            schemeEditorUI.Init();
        }

        public void NewScheme()
        {
            ResetEditor();
            _devices = new List<SchemeDevice>();
            _wires = new List<SchemeDeviceWire>();

            _currentScheme = Scheme.NewScheme();
            _currentCompositionLogicData = _currentScheme.SchemeData.SchemeLogicData as CompositionLogicData;
            _currentSchemeLogicUnit = _currentScheme.InstantiateLogicUnit(-1);

            _gridHandler = EditorDashboard.Instance;
            OnLoadedScheme?.Invoke(_currentScheme);
        }

        // debugOnly: shortcut for SchemeDevice generation
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ProcessNewDevice(InstantiateSchemeDevice(SchemeKey.RELAY, _incrementComponentIndex));
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                ProcessNewDevice(InstantiateSchemeDevice(SchemeKey.ONE_BIT_USER_INPUT, _incrementComponentIndex));
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                ProcessNewDevice(InstantiateSchemeDevice(SchemeKey.ONE_BIT_OUTPUT, _incrementComponentIndex));
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                ProcessNewDevice(InstantiateSchemeDevice(SchemeKey.CONSTANT_VOLTAGE, _incrementComponentIndex));
            }
            
            _currentSchemeLogicUnit.Process();

            // save current scheme
            // if (Input.GetKeyDown(KeyCode.LeftCurlyBracket))
            // {
            //     SaveScheme();
            // }
            
            // load current scheme
            if (Input.GetKeyDown(KeyCode.RightCurlyBracket))
            {
                // loading last 
            }
        }

        #region INSTANTIATION

        // ReSharper disable Unity.PerformanceAnalysis
        private SchemeDevice InstantiateSchemeDevice(SchemeKey schemeKey, int deviceIndex)
        {
            var device = Instantiate(schemeDeviceRef);

            // fixme this _gridHandler is not a useful one
            device.transform.position = _gridHandler.GetMousePositionToGrid();
            var scheme = GameManager.Instance.GetContainerOfType<SchemesContainer>().GetSchemeByKey(schemeKey);
            device.Init(scheme, deviceIndex);
            device.OnDevicePortInteracted += OnDevicePortInteractedHandler;

            // adding movement logic
            var deviceDragDropMovementStrategy = device.AddComponent<DragDropMovementStrategy>();
            var gridSnapMovementExecutionStrategy = new GridSnapMovementExecutionStrategy();
            gridSnapMovementExecutionStrategy.SetGirdHandler(EditorDashboard.Instance);

            IMovementExecutionStrategy movementExecutionStrategy = gridSnapMovementExecutionStrategy;
            deviceDragDropMovementStrategy.SetMovementExecutionStrategy(movementExecutionStrategy);
            deviceDragDropMovementStrategy.SetListOfAcceptableColliders(device.InteractionColliders);
            deviceDragDropMovementStrategy.ShouldTakeIntoAccountInitialDelta = true;
            deviceDragDropMovementStrategy.EnableMovement();

            _devices.Add(device);
            _currentSchemeLogicUnit.AddComponentLogicUnit(scheme.InstantiateLogicUnit(deviceIndex));

            return device;
        }

        private SchemeDevice InstantiateSchemeDevice(ComponentScheme componentScheme,
            ComponentEditorData componentEditorData)
        {
            var device = InstantiateSchemeDevice(componentScheme.SchemeKey, componentScheme.ComponentIndex);
            device.transform.position =
                EditorDashboard.Instance.GetPositionOnGrid(componentEditorData.coordinateOnGrid);
            return device;
        }


        private SchemeDeviceWire InstantiateSchemeDeviceWire()
        {
            var wire = Instantiate(schemeDeviceWireRef);
            return wire;
        }

        private SchemeDeviceWire InstantiateSchemeDeviceWire(WireConnectionEditorData wireConnectionEditorData,
            SchemeRelation schemeRelation)
        {
            var wire = InstantiateSchemeDeviceWire();
            var compositionLogicData = _currentScheme.SchemeData.SchemeLogicData as CompositionLogicData;
            var senderDevice = _devices.First(device =>
                device.DeviceIndex == schemeRelation.senderNode.ComponentIndexInComposition);

            var receiverDevice = _devices.First(device =>
                device.DeviceIndex == schemeRelation.receiverNode.ComponentIndexInComposition);

            var startPort = senderDevice.GetInputPortByIndex(schemeRelation.senderNode.ComponentPortIndex);
            var endPort = receiverDevice.GetOutputPortByIndex(schemeRelation.senderNode.ComponentPortIndex);
            wire.transform.position = startPort.transform.position;
            
            SchemeDeviceWire.ConstructWire(wire, wireConnectionEditorData, startPort, endPort);

            return wire;
        }

        #endregion


        private void OnDevicePortInteractedHandler(SchemeInteractedOnPortsEventArgs arg0)
        {
            if (!_pendingForWireConnection)
            {
                _currentWire = InstantiateSchemeDeviceWire();
                // _currentWire.SetGridHandler(_gridHandler);
                _currentWire.SetPosition(arg0.schemeDevicePortInteractEventArgs.port.transform.position);
                _currentWire.SetStartPort(arg0.schemeDevicePortInteractEventArgs.port);
                _currentWire.StartWiring();
                _currentWire.OnWiringCanceled += OnWiringCanceledHandler;
                _pendingForWireConnection = true;
            }
            else
            {
                if (arg0.schemeDevicePortInteractEventArgs.port.SchemeDevice ==
                    _currentWire.StartPort.SchemeDevice) return;
                if (_currentWire.StartPort.GetType() == arg0.schemeDevicePortInteractEventArgs.port.GetType()) return;

                _pendingForWireConnection = false;
                _currentWire.TerminateActiveWiring();
                _currentWire.SetEndPort(arg0.schemeDevicePortInteractEventArgs.port);
                var relationIndex = DefineRelation(_currentWire.StartPort, _currentWire.EndPort);
                _currentWire.SetRelationIndex(relationIndex);

                _wires.Add(_currentWire);
                SaveWire(_currentWire);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void ProcessNewDevice(SchemeDevice schemeDevice)
        {
            AddDeviceInComposition(schemeDevice);
            SaveDevice(schemeDevice);
        }

        private void OnWiringCanceledHandler()
        {
            Destroy(_currentWire.gameObject);
            _currentWire = null;
            _pendingForWireConnection = false;
        }

        private void AddDeviceInComposition(SchemeDevice schemeDevice)
        {
            ComponentScheme componentScheme = new ComponentScheme(_incrementComponentIndex,
                schemeDevice.UnderliningScheme.SchemeData.SchemeKey);
            _currentCompositionLogicData.ComponentSchemes.Add(componentScheme);

            _incrementComponentIndex++;
        }

        private int DefineRelation(SchemeDevicePort a, SchemeDevicePort b)
        {
            SchemeRelation schemeRelation = new();

            var ports = new List<SchemeDevicePort> { a, b };

            SchemeDevicePort outputPort = ports.First(x => x is SchemeDeviceOutputPort);
            SchemeDevicePort inputPort = ports.First(x => x is SchemeDeviceInputPort);

            // receiver relation node
            schemeRelation.receiverNode =
                new ComponentRelationNode(inputPort.SchemeDevice.DeviceIndex, inputPort.PortIndex);

            // sender relation node
            schemeRelation.senderNode =
                new ComponentRelationNode(outputPort.SchemeDevice.DeviceIndex, outputPort.PortIndex);

            _currentCompositionLogicData.SchemeRelations.Add(schemeRelation);

            return schemeRelation.relationIndex;
        }

        private void SaveDevice(SchemeDevice device)
        {
            _currentScheme.SchemeData.SchemeEditorData.componentEditorDatas.Add(
                new ComponentEditorData(device.DeviceIndex, device.GetCoordinate()));
        }

        private void SaveWire(SchemeDeviceWire wire)
        {
            _currentScheme.SchemeData.SchemeEditorData.wireConnectionEditorDatas.Add(wire.GetConnectionData());
        }

        public void SaveScheme()
        {
            SchemesSaverLoader.SaveScheme(_currentScheme).Forget();
        }

        private void RemoveWire(SchemeDeviceWire wire)
        {
            throw new NotImplementedException();
        }

        private void RemoveRelation(SchemeDevicePort a, SchemeDevicePort b)
        {
            throw new NotImplementedException();
        }

        private void RemoveSchemeDevice(SchemeDevice schemeDevice)
        {
            throw new NotImplementedException();
        }

        private void LoadSchemeInEditor(Scheme scheme)
        {
            _currentScheme = scheme;
            _currentSchemeLogicUnit = scheme.InstantiateLogicUnit(-1);

            _devices ??= new List<SchemeDevice>();
            _devices.Clear();

            _wires ??= new List<SchemeDeviceWire>();
            _wires.Clear();
            
            var compositionLogicData = scheme.SchemeData.SchemeLogicData as CompositionLogicData;
            if (compositionLogicData == null)
            {
                throw new GameLogicException("Trying to edit scheme that is not composition.");
            }

            var schemeEditorData = scheme.SchemeData.SchemeEditorData;
            var componentEditorDatas = schemeEditorData.componentEditorDatas;

            foreach (var componentScheme in compositionLogicData.ComponentSchemes)
            {
                var componentEditorData =
                    componentEditorDatas.First(x => x.componentIndex == componentScheme.ComponentIndex);
                var device = InstantiateSchemeDevice(componentScheme, componentEditorData);
                _devices.Add(device);
            }

            _incrementComponentIndex = compositionLogicData.ComponentSchemes.Max(x => x.ComponentIndex);

            foreach (var relation in compositionLogicData.SchemeRelations)
            {
                var wireConnectionEditorData =
                    schemeEditorData.wireConnectionEditorDatas.First(x => x.relationIndex == relation.relationIndex);
                var wire = InstantiateSchemeDeviceWire(wireConnectionEditorData, relation);
                _wires.Add(wire);
            }
            
            OnLoadedScheme?.Invoke(_currentScheme);
        }

        public void ResetEditor()
        {
            _currentSchemeLogicUnit = null;
            _devices?.ForEach(device=>Destroy(device.gameObject));
            _wires?.ForEach(wire => Destroy(wire.gameObject));
        }

        public SchemeDevice GetSchemeDeviceReference()
        {
            return schemeDeviceRef;
        }
    }
}