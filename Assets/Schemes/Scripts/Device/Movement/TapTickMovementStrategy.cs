using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Exceptions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Schemes.Device.Movement
{
    public class TapTickMovementStrategy : MonoBehaviour, IMovementStrategy, IPointerClickHandler
    {
        public IMovementExecutionStrategy MovementExecutionStrategy { get; set; }
        
        public event UnityAction OnTap;
        public event UnityAction OnTick;

        public void SetMovementExecutionStrategy(IMovementExecutionStrategy movementExecutionStrategy)
        {
            MovementExecutionStrategy = movementExecutionStrategy;
        }
        
        private List<GameObject> _mouseInteractableGameObjects;
        private bool _movementEnabled;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _inMovement;
        public void EnableMovement()
        {
            _movementEnabled = true;
        }

        public void DisableMovement()
        {
            _movementEnabled = true;
        }
        
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!_movementEnabled) return; 
            if (!ValidatePointerEventSelectedObject(eventData.pointerPressRaycast.gameObject)) return;
            if(!Input.GetKeyUp(KeyCode.Mouse0)) return;
            
            if (_inMovement)
            {
                Tick();
            }
            else
            {
                Tap();
            }
        }
        
        public void Tap()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _inMovement = true;
            MouseFollowMovement(_cancellationTokenSource.Token).Forget();
            OnTap?.Invoke();
        }

        public void Tick()
        {
            _cancellationTokenSource?.Cancel();
            _inMovement = false;
            OnTick?.Invoke();
        }
        
        public async UniTask MouseFollowMovement(CancellationToken cancellationToken)
        {
            var startMousePosition = Input.mousePosition;
            Plane planeOnWhichMoves = new Plane(transform.up, transform.position);
            Vector3 hitPosition = GetPositionOnMovementPlane(planeOnWhichMoves, startMousePosition);
            while (true)
            {
                var currentMousePosition = Input.mousePosition;
                var totalDelta = startMousePosition - currentMousePosition;

                var positionOnMovementPlane = GetPositionOnMovementPlane(planeOnWhichMoves, currentMousePosition);
                var anticipatedPosition = positionOnMovementPlane;
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
        
        private void OnDestroy()
        {
            Tick();
        }
    }
}