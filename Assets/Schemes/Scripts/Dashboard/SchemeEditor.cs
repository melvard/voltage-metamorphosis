using System;
using System.Collections.Generic;
using System.Linq;
using Canvas;
using Cysharp.Threading.Tasks;
using Exceptions;
using GameLogic;
using Schemes.Data;
using Schemes.Data.LogicData;
using Schemes.Data.LogicData.Composition;
using Schemes.Data.LogicData.UserIO;
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

        [Required] [AssetsOnly] [SerializeField] private SchemeDevice schemeDeviceRef;
        [Required] [AssetsOnly] [SerializeField] private SchemeDeviceWire schemeDeviceWireRef;

        #endregion

        #region PRIVATE_FIELDS

        [DisableInPlayMode] [DisableInEditorMode] [ShowInInspector]
        private Scheme _currentScheme;

        private SchemeLogicUnit _currentSchemeLogicUnit;
        private CompositionLogicData _currentCompositionLogicData;
        private SchemeDeviceWire _currentWire;

        [DisableInPlayMode] [DisableInEditorMode] [ShowInInspector]
        private List<SchemeDevice> _devices;
        [DisableInPlayMode] [DisableInEditorMode] [ShowInInspector]
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
        }

        public Scheme NewScheme()
        {
            ResetEditor();
            _devices = new List<SchemeDevice>();
            _wires = new List<SchemeDeviceWire>();

            _currentScheme = Scheme.NewScheme();
            _currentCompositionLogicData = _currentScheme.SchemeData.SchemeLogicData as CompositionLogicData;
            _currentSchemeLogicUnit = new SchemeLogicUnit(_currentScheme.SchemeData, -1);
            _currentSchemeLogicUnit.AlignInputsAndOutputsOnComponents();
            
            _gridHandler = EditorDashboard.Instance;
            OnLoadedScheme?.Invoke(_currentScheme);
            return _currentScheme;
        }

        // todo: actively processing schemeLogic unit is not a good option, consider implementing edge triggered function
        private void Update()
        {
            if (_currentSchemeLogicUnit != null)
            {
                // if (Input.GetKey(KeyCode.Space))
                // {
                //    
                // }
                _currentSchemeLogicUnit.Process();
                UpdateWireValues();
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
            device.OnDeviceRemoveCommand += OnDeviceRemoveCommandHandler;
            
            // adding movement logic
            var deviceDragDropMovementStrategy = device.AddComponent<DragDropMovementStrategy>();
            var gridSnapMovementExecutionStrategy = new GridSnapMovementExecutionStrategy();
            gridSnapMovementExecutionStrategy.SetGirdHandler(EditorDashboard.Instance);

            IMovementExecutionStrategy movementExecutionStrategy = gridSnapMovementExecutionStrategy;
            deviceDragDropMovementStrategy.SetMovementExecutionStrategy(movementExecutionStrategy);
            deviceDragDropMovementStrategy.SetListOfAcceptableColliders(device.InteractionColliders);
            deviceDragDropMovementStrategy.ShouldTakeIntoAccountInitialDelta = true;
            deviceDragDropMovementStrategy.EnableMovement();

            return device;
        }

        private void OnDeviceRemoveCommandHandler(SchemeDevice device)
        {
            RemoveSchemeDevice(device);
        }

        private void OnWireRemoveCommandHandler(SchemeDeviceWire wire)
        {
            RemoveSchemeWire(wire);
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
            wire.OnWireRemoveCommand += OnWireRemoveCommandHandler;
            return wire;
        }
        
        private SchemeDeviceWire InstantiateSchemeDeviceWire(WireConnectionEditorData wireConnectionEditorData,
            SchemeRelation schemeRelation)
        {
            var wire = InstantiateSchemeDeviceWire();
            var senderDevice = _devices.First(device =>
                device.DeviceIndex == schemeRelation.senderNode.ComponentIndexInComposition);

            var receiverDevice = _devices.First(device =>
                device.DeviceIndex == schemeRelation.receiverNode.ComponentIndexInComposition);

            var startPort = senderDevice.GetOutputPortByIndex(schemeRelation.senderNode.ComponentPortIndex);
            var endPort = receiverDevice.GetInputPortByIndex(schemeRelation.receiverNode.ComponentPortIndex);
            
            wire.transform.position = startPort.transform.position;

            wire.ConstructWire(wireConnectionEditorData, startPort, endPort);

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
                _currentWire.ArrangePorts();
                DefineRelation(_currentWire.StartPort, _currentWire.EndPort, _incrementRelationIndex);
                _currentWire.SetRelationIndex(_incrementRelationIndex);
                _incrementRelationIndex++;

                _wires.Add(_currentWire);
            }
        }

        private void ProcessNewDevice(SchemeDevice schemeDevice)
        {
            _devices.Add(schemeDevice);
            AddDeviceInComposition(schemeDevice);
            var schemeLogicUnit = new SchemeLogicUnit(schemeDevice.UnderliningScheme.SchemeData,
                schemeDevice.DeviceIndex);
            _currentSchemeLogicUnit.AddComponentLogicUnit(schemeLogicUnit);
            schemeLogicUnit.AlignInputsAndOutputsOnComponents();
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

        private void DefineRelation(SchemeDevicePort outputPort, SchemeDevicePort inputPort, int relationIndex)
        {
            SchemeRelation schemeRelation = new();

            // receiver relation node
            schemeRelation.receiverNode =
                new ComponentRelationNode(inputPort.SchemeDevice.DeviceIndex, inputPort.PortIndex);

            // sender relation node
            schemeRelation.senderNode =
                new ComponentRelationNode(outputPort.SchemeDevice.DeviceIndex, outputPort.PortIndex);

            schemeRelation.relationIndex = relationIndex;
            _currentCompositionLogicData.SchemeRelations.Add(schemeRelation);
        }
        
        public Scheme PreapareSchemeForSave(SchemeUIData schemeUIData)
        {
            _currentScheme.SchemeData.SchemeEditorData.wireConnectionEditorDatas = new();
            _currentScheme.SchemeData.SchemeEditorData.componentEditorDatas = new();
            _currentScheme.SchemeData.SchemeEditorData.inputEditorDatas = new ();
            _currentScheme.SchemeData.SchemeEditorData.outputEditorDatas = new ();

            foreach (var schemeDevice in _devices)
            {
                _currentScheme.SchemeData.SchemeEditorData.componentEditorDatas.Add(
                    new ComponentEditorData(schemeDevice.DeviceIndex, schemeDevice.GetCoordinate()));
            }

            foreach (var wire in _wires)
            {
                _currentScheme.SchemeData.SchemeEditorData.wireConnectionEditorDatas.Add(wire.GetConnectionData());
            }
            
            _currentScheme.SchemeData.UpdateDataFromUI(schemeUIData);

            // define number and order of inputs of scheme
            if (_currentScheme.SchemeData.SchemeLogicData is IInputPortSchemesLogicData inputPortSchemesLogicData)
            {
                inputPortSchemesLogicData.NumberOfInputs =
                    (byte)_devices.Count(x => x.UnderliningScheme.SchemeKey == SchemeKey.ONE_BIT_USER_INPUT);
                _currentScheme.SchemeData.SchemeVisualsData.ArrangeInputPositionAutomatically(inputPortSchemesLogicData.NumberOfInputs);
                int numberOfInput = 0;
                foreach (var device in _devices)
                {
                    // if current device is UserInput device then effectively add information about component index
                    if (device.UnderliningScheme.SchemeData.SchemeLogicData is UserInputLogicData)
                    {
                        _currentScheme.SchemeData.SchemeEditorData.inputEditorDatas.Add(
                            new IOEditorData(device.DeviceIndex, numberOfInput));
                        numberOfInput++;
                    }
                } 
            }

            // define number and order of outputs of scheme
            if (_currentScheme.SchemeData.SchemeLogicData is IOutputPortSchemeLogicData outputPortSchemeLogicData)
            {
                outputPortSchemeLogicData.NumberOfOutputs =
                    (byte)_devices.Count(x => x.UnderliningScheme.SchemeKey == SchemeKey.ONE_BIT_OUTPUT);
                _currentScheme.SchemeData.SchemeVisualsData.ArrangeOutputPositionAutomatically(outputPortSchemeLogicData.NumberOfOutputs);
                
                int numberOfOutput = 0;
                foreach (var device in _devices)
                {
                    // if current device is UserOutput device then effectively add information about component index
                    if (device.UnderliningScheme.SchemeData.SchemeLogicData is UserOutputLogicData)
                    {
                        _currentScheme.SchemeData.SchemeEditorData.outputEditorDatas.Add(
                            new IOEditorData(device.DeviceIndex, (byte)numberOfOutput));
                        numberOfOutput++;
                    }
                }
            }

            return _currentScheme;
        }

        private void UpdateWireValues()
        {
            foreach (var schemeDeviceWire in _wires)
            {
                var outputLogicUnit = _currentSchemeLogicUnit.ComponentLogicUnits.First(x =>
                    x.index == schemeDeviceWire.StartPort.SchemeDevice.DeviceIndex);
                var value = outputLogicUnit.Outputs[schemeDeviceWire.StartPort.PortIndex].Value;
                schemeDeviceWire.UpdateWireValue(value);
            } 
        }
        
        private void RemoveSchemeWire(SchemeDeviceWire wire)
        {
            var relationsWithWire = _currentCompositionLogicData.SchemeRelations
                .Where(relation=> relation.relationIndex == wire.GetRelationIndex()).ToList();

            for (var i = 0; i < _wires.Count; i++)
            {
                if (_wires[i].GetRelationIndex() == wire.GetRelationIndex())
                {
                    _wires[i].DestroyCommand();
                    _wires.RemoveAt(i);
                    i--;
                }
            }
            
            _currentCompositionLogicData.ThrowRelationsWithIndices(relationsWithWire.Select(x=>x.relationIndex).ToArray());

        }
        
        private void RemoveSchemeDevice(SchemeDevice device)
        {
            var indexOf = _devices.IndexOf(device);
            if (indexOf == -1)
            {
                Destroy(device.gameObject);
                return;
            };

            var relationsWithDevice = _currentCompositionLogicData.SchemeRelations
                    .Where(x=>
                    {
                        var isDevicePort = x.senderNode.ComponentIndexInComposition == device.DeviceIndex;
                        var isToDevicePort = x.receiverNode.ComponentIndexInComposition == device.DeviceIndex;

                        return isDevicePort || isToDevicePort;
                    }).ToList();

            if (relationsWithDevice.Count != 0)
            {
                for (var i = 0; i < _wires.Count; i++)
                {
                    var schemeDeviceWire = _wires[i];
                    if (relationsWithDevice.Any(relation => relation.relationIndex == schemeDeviceWire.GetRelationIndex()))
                    {
                        _wires[i].DestroyCommand();
                        _wires.RemoveAt(i);
                        i--;
                    }
                }

                _currentCompositionLogicData.ThrowRelationsWithIndices(relationsWithDevice.Select(x=>x.relationIndex).ToArray());
                
            }

            for (var i = 0; i < _currentCompositionLogicData.ComponentSchemes.Count; i++)
            {
                if (_currentCompositionLogicData.ComponentSchemes[i].ComponentIndex == device.DeviceIndex)
                {
                    _currentCompositionLogicData.ComponentSchemes.RemoveAt(i);
                }
            }

            _currentSchemeLogicUnit.RemoveComponentLogicUnitWithIndex(device.DeviceIndex);
            _devices.RemoveAt(indexOf);
            Destroy(device.gameObject);
        }

        public void LoadSchemeInEditor(Scheme scheme)
        {
            _currentScheme = Scheme.CopyFrom(scheme);
            _currentCompositionLogicData = _currentScheme.SchemeData.SchemeLogicData as CompositionLogicData;
            _currentSchemeLogicUnit = new SchemeLogicUnit(_currentScheme.SchemeData, -1);
            _currentSchemeLogicUnit.AlignInputsAndOutputsOnComponents();

            _devices ??= new List<SchemeDevice>();
            _devices.Clear();

            _wires ??= new List<SchemeDeviceWire>();
            _wires.Clear();

            var compositionLogicData = _currentScheme.SchemeData.SchemeLogicData as CompositionLogicData;
            if (compositionLogicData == null)
            {
                throw new GameLogicException("Trying to edit scheme that is not composition.");
            }

            var schemeEditorData = _currentScheme.SchemeData.SchemeEditorData;
            var componentEditorDatas = schemeEditorData.componentEditorDatas;

            foreach (var componentScheme in compositionLogicData.ComponentSchemes)
            {
                var componentEditorData =
                    componentEditorDatas.First(x => x.componentIndex == componentScheme.ComponentIndex);
                var device = InstantiateSchemeDevice(componentScheme, componentEditorData);
                _devices.Add(device);
                // _currentSchemeLogicUnit.AddComponentLogicUnit(new SchemeLogicUnit(device.UnderliningScheme.SchemeData, device.DeviceIndex));
            }

            _incrementComponentIndex = compositionLogicData.ComponentSchemes.Count != 0 ? compositionLogicData.ComponentSchemes.Max(x => x.ComponentIndex) + 1 : 0;

            foreach (var relation in compositionLogicData.SchemeRelations)
            {
                var wireConnectionEditorData =
                    schemeEditorData.wireConnectionEditorDatas.First(x => x.relationIndex == relation.relationIndex);
                var wire = InstantiateSchemeDeviceWire(wireConnectionEditorData, relation);
                _wires.Add(wire);
            }

            _incrementRelationIndex = compositionLogicData.SchemeRelations.Count != 0 ? compositionLogicData.SchemeRelations.Max(x => x.relationIndex) + 1 : 0;

            OnLoadedScheme?.Invoke(_currentScheme);
        }
        
        public void ResetEditor()
        {
            _currentSchemeLogicUnit = null;
            _devices?.ForEach(device => Destroy(device.gameObject));
            _wires?.ForEach(wire => Destroy(wire.gameObject));
            _devices?.Clear();
            _wires?.Clear();
        }

        public void ClearComponentsAndWires()
        {
            if (_currentScheme == null)
                throw new GameLogicException(
                    "It is not possible to clear components of scheme that is not loaded into scheme editor");
            // todo: remove component and relation datas 
            
            _currentSchemeLogicUnit = new SchemeLogicUnit(_currentScheme.SchemeData, -1);
            _currentSchemeLogicUnit.AlignInputsAndOutputsOnComponents();

            for (var i = 0; i < _devices.Count; i++)
            {
                RemoveSchemeDevice(_devices[i]);
                i--;
            }
            
            for (var i = 0; i < _wires.Count; i++)
            {
                RemoveSchemeWire(_wires[i]);
                i--;
            }
            
            _devices?.Clear();
            _wires?.Clear();
        }

        public SchemeDevice GetSchemeDeviceReference()
        {
            return schemeDeviceRef;
        }

        public async UniTask GenerateDevice(Scheme scheme)
        {
            var device = InstantiateSchemeDevice(scheme.SchemeData.SchemeKey, _incrementComponentIndex);
            ProcessNewDevice(device);
            var tapTickMovementStrategy = device.AddComponent<TapTickMovementStrategy>();
            var gridSnapMovementExecutionStrategy = new GridSnapMovementExecutionStrategy();
            gridSnapMovementExecutionStrategy.SetGirdHandler(EditorDashboard.Instance);

            tapTickMovementStrategy.SetMovementExecutionStrategy(gridSnapMovementExecutionStrategy);
            tapTickMovementStrategy.SetListOfAcceptableColliders(device.InteractionColliders);
            tapTickMovementStrategy.EnableMovement();
            tapTickMovementStrategy.Tap();

            bool placedDevice = false;
            tapTickMovementStrategy.OnTick += () =>
            {
                placedDevice = true;
                Destroy(tapTickMovementStrategy);
            };

            await UniTask.WaitUntil(() => device == null || placedDevice);
        }
    }
}