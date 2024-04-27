using System;
using System.Collections.Generic;
using Schemes.Device;
using Schemes.Device.Movement;
using UnityEngine;

namespace GameCamera
{
    public class GameCameraMovementController : MonoBehaviour
    {
        private List<IMovementStrategy> _movementStrategies;

        private void Awake()
        {
            _movementStrategies = new();

            AddNavigationMovement();
            AddZoomInOutMovement();
            
            foreach (var movementStrategy in _movementStrategies)
            {
                movementStrategy.EnableMovement();
            }
        }
        
        

        // fixme: this code can't be here: CameraMovementController should not choose strategy
        private void AddNavigationMovement()
        {
            var wasdMovementStrategy = gameObject.AddComponent<WASDMovementStrategy>();
            IMovementExecutionStrategy movementExecutionStrategy = new PoorMovementExecutionStrategy();
            wasdMovementStrategy.SetMovementExecutionStrategy(movementExecutionStrategy);
            wasdMovementStrategy.VerticalMovementSpeed = 0.1f;
            wasdMovementStrategy.HorizontalMovementSpeed = 0.1f;
            wasdMovementStrategy.MovementBoostKey = KeyCode.LeftShift;
            wasdMovementStrategy.MovementBoostMultiplier = 2f;
            
            _movementStrategies.Add(wasdMovementStrategy);
        }
        
        // fixme: this code can't be here: CameraMovementController should not choose strategy
        private void AddZoomInOutMovement()
        {
            var scrollUpDownMovement = gameObject.AddComponent<ScrollUpDownMovementStrategy>();
            IMovementExecutionStrategy movementExecutionStrategy = new PoorMovementExecutionStrategy();
            scrollUpDownMovement.SetMovementExecutionStrategy(movementExecutionStrategy);
            scrollUpDownMovement.ScrollSpeed = 3f;
            scrollUpDownMovement.MinY = -5;
            scrollUpDownMovement.MaxY = 100;
            scrollUpDownMovement.EnableMovement();
            _movementStrategies.Add(scrollUpDownMovement);
        }
        
    }
}