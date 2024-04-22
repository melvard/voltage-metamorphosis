using Schemes.Device.Movement;

namespace Schemes.Device
{
    public interface IMovementStrategy
    {
        IMovementExecutionStrategy MovementExecutionStrategy { get; set;}
        public void SetMovementExecutionStrategy(IMovementExecutionStrategy movementExecutionStrategy);
        public void EnableMovement();
        public void DisableMovement();
    }
}