using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using GameLogic;
using Schemes.Device;
using Schemes.Device.Movement;

namespace GameCamera
{
    public class ScrollUpDownMovementStrategy : MonoBehaviour, IMovementStrategy
    {
        public IMovementExecutionStrategy MovementExecutionStrategy { get; set; }
        private CancellationTokenSource _movementCancellationSource;
        public float ScrollSpeed { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }

        public void SetMovementExecutionStrategy(IMovementExecutionStrategy movementExecutionStrategy)
        {
            MovementExecutionStrategy = movementExecutionStrategy;
        }

        private async UniTask ScrollUpAndDown(CancellationToken cancellationToken)
        {
            while (true)
            {
                float scrollInput = InputsManager.GetAxis("Mouse ScrollWheel", gameObject.layer);
                var currentPosition = transform.position;

                // Calculate new Y position
                float newY = currentPosition.y + scrollInput * ScrollSpeed;
                // Clamp Y position within range
                newY = Mathf.Clamp(newY, MinY, MaxY);

                // Update position with new Y value
                currentPosition.y = newY;

                MovementExecutionStrategy.SetAnticipatedPosition(transform, currentPosition);

                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.Yield(cancellationToken);
            }
        }

        public void EnableMovement()
        {
            _movementCancellationSource = new CancellationTokenSource();
            ScrollUpAndDown(_movementCancellationSource.Token).Forget();
        }

        public void DisableMovement()
        {
            if (_movementCancellationSource != null) _movementCancellationSource.Cancel();
        }

    }
}