using GameLogic;
using Schemes;
using Schemes.Dashboard;
using Schemes.Data;
using Schemes.Device;
using Schemes.Device.Movement;
using Schemes.Device.Ports;
using Schemes.Device.Wire;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class SchemeEditor : MonoBehaviour
{
    public const string RELAY_KEY = "d3336114-5910-42bd-b5e2-79518d3ff893";
    public const string ONE_BIT_USER_INPUT_KEY = "2b738a45-ac8c-4508-8385-84491fe01b62";
    public const string ONE_BIT_OUTPUT_KEY= "6c6dabff-4878-4697-8001-9ec948eed7cd";
    public const string CONSTANT_VOLTAGE_KEY = "ee5cf222-b539-4e0c-a5b6-179a9561825d";
    
    public SchemeDevice schemeDeviceRef;
    public SchemeDeviceWire schemeDeviceWireRef;

    [DisableInPlayMode][DisableInEditorMode][ShowInInspector] private Scheme _currentScheme;
    private CompositionLogicData _currentCompositionLogicData;

    private SchemeDeviceWire _currentWire;
    private IGridHandler _gridHandler;
    
    private void Start()
    {
        _currentScheme = new Scheme();
        _currentCompositionLogicData = _currentScheme.SchemeData.SchemeLogicData as CompositionLogicData;
        _gridHandler = EditorDashboard.Instance;
        
    }
    
    // hack: shortcut for SchemeDevice generation
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            AddDeviceInComposition(GenerateSchemeDevice(RELAY_KEY));
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddDeviceInComposition(GenerateSchemeDevice(ONE_BIT_USER_INPUT_KEY));
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            AddDeviceInComposition(GenerateSchemeDevice(ONE_BIT_OUTPUT_KEY));
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            AddDeviceInComposition(GenerateSchemeDevice(CONSTANT_VOLTAGE_KEY));
        }
    }
    

    // ReSharper disable Unity.PerformanceAnalysis
    private SchemeDevice GenerateSchemeDevice(string schemeKey)
    {
        var schemeDevice = Instantiate(schemeDeviceRef);
        schemeDevice.transform.position = _gridHandler.GetPositionOnGridWithMouse();
        
        schemeDevice.Init(GameManager.Instance.GetContainerOfType<SchemesContainer>().GetSchemeByKey(schemeKey));
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
        return schemeDevice;
    }

    private bool _pendingForWireConnection = false;
    private void OnDevicePortInteractedHandler(SchemeInteractedOnPortsEventArgs arg0)
    {
        if (!_pendingForWireConnection)
        {
            _currentWire = Instantiate(schemeDeviceWireRef);
            _currentWire.Init();
            // _currentWire.SetGridHandler(_gridHandler);
            _currentWire.SetPosition(arg0.schemeDevicePortInteractEventArgs.port.transform.position);
            _currentWire.SetStartPort(arg0.schemeDevicePortInteractEventArgs.port);
            _currentWire.StartWiring();
            _pendingForWireConnection = true;
        }
        else
        {
            _currentWire.StopWiring();
            _currentWire.SetEndPort(arg0.schemeDevicePortInteractEventArgs.port);
            DefineRelation(_currentWire.StartPort, _currentWire.EndPort);
        }
    }

   
    private void AddDeviceInComposition(SchemeDevice schemeDevice)
    {
        ComponentScheme componentScheme = new ComponentScheme(0, schemeDevice.UnderliningScheme.SchemeData.SchemeKey);
        _currentCompositionLogicData.ComponentSchemes.Add(componentScheme);
    }


    // idk what the hell is this going to do
    private void DefineRelation(SchemeDevicePort a, SchemeDevicePort b)
    {
        SchemeRelation schemeRelation = new();
        
        _currentCompositionLogicData.SchemeRelations.Add(schemeRelation);
    }
    
    
}
