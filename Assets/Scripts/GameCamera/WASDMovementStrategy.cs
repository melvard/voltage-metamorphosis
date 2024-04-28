using System.Threading;
using Cysharp.Threading.Tasks;
using Schemes.Device;
using Schemes.Device.Movement;
using UnityEngine;

namespace GameCamera
{
    public class WASDMovementStrategy : MonoBehaviour, IMovementStrategy
    {
        public IMovementExecutionStrategy MovementExecutionStrategy { get; set; }
        private CancellationTokenSource _camMovementTasksCancellationSource;
        public float VerticalMovementSpeed { get; set; }
        public float HorizontalMovementSpeed { get; set; }

        public KeyCode MovementBoostKey { get; set; }
        public float MovementBoostMultiplier { get; set; }

        public void SetMovementExecutionStrategy(IMovementExecutionStrategy movementExecutionStrategy)
        {
            MovementExecutionStrategy = movementExecutionStrategy;
        }
        private async UniTask CamWASDMovement(CancellationToken cancellationToken)
        {
            
            while (true)
            {
                float verticalInput = Input.GetAxis("Vertical");
                float horizontalInput = Input.GetAxis("Horizontal");

                float boostMultiplier = 1f;
                if (Input.GetKey(MovementBoostKey))
                {
                    boostMultiplier = MovementBoostMultiplier; 
                }

                verticalInput *= boostMultiplier;
                horizontalInput *= boostMultiplier;
                
                var currentPosition = transform.position;
                currentPosition += (verticalInput * VerticalMovementSpeed) * Vector3.forward;
                currentPosition += (horizontalInput * HorizontalMovementSpeed) * Vector3.right;
                MovementExecutionStrategy.SetAnticipatedPosition(transform, currentPosition);
                
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.Yield(cancellationToken);
            }
        }

        public void EnableMovement()
        {
            _camMovementTasksCancellationSource = new CancellationTokenSource();
            CamWASDMovement(_camMovementTasksCancellationSource.Token).Forget();
        }

        public void DisableMovement()
        {
           if(_camMovementTasksCancellationSource != null) _camMovementTasksCancellationSource.Cancel();
           
        }
    }
}