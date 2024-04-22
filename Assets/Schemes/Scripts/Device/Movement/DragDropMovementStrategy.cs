using System.Threading;
using Cysharp.Threading.Tasks;
using Exceptions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Schemes.Device.Movement
{
    public class DragDropMovementStrategy : MonoBehaviour, IMovementStrategy, IPointerDownHandler, IPointerUpHandler
    {
        public IMovementExecutionStrategy MovementExecutionStrategy { get; set; }

        private CancellationTokenSource _cancellationTokenSource;
        
        private bool _movementEnabled;
        
        public void SetMovementExecutionStrategy(IMovementExecutionStrategy movementExecutionStrategy)
        {
            MovementExecutionStrategy = movementExecutionStrategy;
        }

        public void EnableMovement()
        {
            _movementEnabled = true;
        }

        public void DisableMovement()
        {
            _movementEnabled = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!_movementEnabled) return;
            _cancellationTokenSource = new CancellationTokenSource();
            DragDropMovementHandler(_cancellationTokenSource.Token).Forget();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(!_movementEnabled) return;
            
            if(_cancellationTokenSource != null) _cancellationTokenSource.Cancel();
        }

        public async UniTask DragDropMovementHandler(CancellationToken cancellationToken)
        {
            var startMousePosition = Input.mousePosition;
            Plane planeOnWhichMoves = new Plane(transform.up, transform.position);
            Vector3 hitPosition = GetPositionOnMovementPlane(planeOnWhichMoves, startMousePosition);
            var initialDeltaRelativeToBody = hitPosition - transform.position;
            
            while (true)
            {
                var currentMousePosition = Input.mousePosition;
                var positionOnMovementPlane = GetPositionOnMovementPlane(planeOnWhichMoves, currentMousePosition);
                var anticipatedPosition = positionOnMovementPlane - initialDeltaRelativeToBody;
                MovementExecutionStrategy.SetAnticipatedPosition(transform, anticipatedPosition);
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.Yield(cancellationToken);
            }
        }   

        private Vector3 GetPositionOnMovementPlane(Plane plane, Vector3 mouseInput)
        {
            Ray ray = Camera.main.ScreenPointToRay(mouseInput);
            if (plane.Raycast(ray, out var distance))
            {
                return ray.GetPoint(distance);
            }

            throw new GameLogicException("Could not hit the plane you are trying to cast on for drag drop movement. Such behaviour is not expected.");
        }

    }
}