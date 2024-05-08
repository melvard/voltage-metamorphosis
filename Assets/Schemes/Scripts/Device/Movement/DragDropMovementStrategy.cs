using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Exceptions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Schemes.Device.Movement
{
    public class DragDropMovementStrategy : MonoBehaviour, IMovementStrategy, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler
    {
        public IMovementExecutionStrategy MovementExecutionStrategy { get; set; }
        public bool ShouldTakeIntoAccountInitialDelta { get; set; }
        public float movementDeltaThreshold;

        private CancellationTokenSource _cancellationTokenSource;
        
       
        private bool _movementEnabled;
        private List<GameObject> _mouseInteractableGameObjects;
        
        public void SetMovementExecutionStrategy(IMovementExecutionStrategy movementExecutionStrategy)
        {
            MovementExecutionStrategy = movementExecutionStrategy;
        }

        public void SetListOfAcceptableColliders(List<Collider> mouseInteractableColliders)
        {
            _mouseInteractableGameObjects = mouseInteractableColliders.Select(x => x.gameObject).ToList();
        }
        
        private bool ValidatePointerEventSelectedObject(GameObject go)
        {
            foreach (var mouseInteractableGameObject in _mouseInteractableGameObjects)
            {
                if (mouseInteractableGameObject.gameObject == go) return true;
            }

            return false;
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
            if (!_movementEnabled) return;
            if (!ValidatePointerEventSelectedObject(eventData.pointerPressRaycast.gameObject)) return;
            
            _cancellationTokenSource = new CancellationTokenSource();
            DragDropMovementHandler(_cancellationTokenSource.Token).Forget();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(!_movementEnabled) return;
            if (!ValidatePointerEventSelectedObject(eventData.pointerPressRaycast.gameObject)) return;

            if(_cancellationTokenSource != null) _cancellationTokenSource.Cancel();
        }

        private async UniTask DragDropMovementHandler(CancellationToken cancellationToken)
        {
            var startMousePosition = Input.mousePosition;
            Plane planeOnWhichMoves = new Plane(transform.up, transform.position);
            Vector3 hitPosition = GetPositionOnMovementPlane(planeOnWhichMoves, startMousePosition);
            
            var initialDeltaRelativeToBody = Vector3.zero;
            if (ShouldTakeIntoAccountInitialDelta)
            {
                initialDeltaRelativeToBody = hitPosition - transform.position;
            }

            bool overcameThreshold = false;
            while (true)
            {
                var currentMousePosition = Input.mousePosition;
                var totalDelta = startMousePosition - currentMousePosition;
                if (totalDelta.magnitude > movementDeltaThreshold)
                {
                    overcameThreshold = true;
                }

                if (overcameThreshold)
                {
                    var positionOnMovementPlane = GetPositionOnMovementPlane(planeOnWhichMoves, currentMousePosition);
                    var anticipatedPosition = positionOnMovementPlane - initialDeltaRelativeToBody;
                    MovementExecutionStrategy.SetAnticipatedPosition(transform, anticipatedPosition);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                await UniTask.Yield(cancellationToken);
            }
        }   

        private Vector3 GetPositionOnMovementPlane(Plane plane, Vector3 mouseInput)
        {
            var ray = Camera.main.ScreenPointToRay(mouseInput);
            if (plane.Raycast(ray, out var distance))
            {
                return ray.GetPoint(distance);
            }

            throw new GameLogicException("Could not hit the plane you are trying to cast on for drag drop movement. Such behaviour is not expected.");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log(eventData.selectedObject);
        }
    }
}