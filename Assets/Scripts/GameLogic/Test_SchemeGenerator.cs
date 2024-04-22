using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Schemes;
using Schemes.Device;
using Schemes.Device.Movement;
using Unity.VisualScripting;
using UnityEngine;

public class Test_SchemeGenerator : MonoBehaviour
{
    public SchemeDevice schemeDeviceRef;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            var schemeDevice = Instantiate(schemeDeviceRef);
            schemeDevice.Init(GameManager.Instance.GetContainerOfType<SchemesContainer>().GetSchemeByKey("709f7472-8837-4836-9cec-71e9854bb878"));
            
            var deviceDragDropMovementStrategy = schemeDevice.AddComponent<DragDropMovementStrategy>();
            IMovementExecutionStrategy movementExecutionStrategy = new PoorMovementExecutionStrategy();
            deviceDragDropMovementStrategy.SetMovementExecutionStrategy(movementExecutionStrategy);
            deviceDragDropMovementStrategy.EnableMovement();
        }
    }
}
