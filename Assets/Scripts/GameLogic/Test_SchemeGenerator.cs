using System.Net.Mime;
using GameLogic;
using Schemes;
using Schemes.Device;
using Schemes.Device.Movement;
using Unity.VisualScripting;
using UnityEngine;

public class Test_SchemeGenerator : MonoBehaviour
{
    public const string RELAY_KEY = "d3336114-5910-42bd-b5e2-79518d3ff893";
    public const string ONE_BIT_USER_INPUT_KEY = "2b738a45-ac8c-4508-8385-84491fe01b62";
    public const string ONE_BIT_OUTPUT_KEY= "6c6dabff-4878-4697-8001-9ec948eed7cd";
    public const string CONSTANT_VOLTAGE_KEY = "ee5cf222-b539-4e0c-a5b6-179a9561825d";
    public SchemeDevice schemeDeviceRef;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateSchemeDevice(RELAY_KEY);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            GenerateSchemeDevice(ONE_BIT_USER_INPUT_KEY);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            GenerateSchemeDevice(ONE_BIT_OUTPUT_KEY);
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            GenerateSchemeDevice(CONSTANT_VOLTAGE_KEY);
        }
    }
    private void GenerateSchemeDevice(string schemeKey)
    {
        var schemeDevice = Instantiate(schemeDeviceRef);
        schemeDevice.transform.position = GetOnBoardPos();
        schemeDevice.Init(GameManager.Instance.GetContainerOfType<SchemesContainer>().GetSchemeByKey(schemeKey));
            
        var deviceDragDropMovementStrategy = schemeDevice.AddComponent<DragDropMovementStrategy>();
        IMovementExecutionStrategy movementExecutionStrategy = new PoorMovementExecutionStrategy();
        deviceDragDropMovementStrategy.SetMovementExecutionStrategy(movementExecutionStrategy);
        deviceDragDropMovementStrategy.EnableMovement();
    }

    private Vector3 GetOnBoardPos()
    {
        Plane plane = new Plane(Vector3.up, 0f);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }
}
