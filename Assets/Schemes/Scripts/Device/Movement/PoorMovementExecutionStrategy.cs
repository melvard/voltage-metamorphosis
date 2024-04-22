using UnityEngine;

namespace Schemes.Device.Movement
{
    public class PoorMovementExecutionStrategy : IMovementExecutionStrategy
    {
        public void SetAnticipatedPosition(Transform moveable, Vector3 anticipatedPosition)
        {
            moveable.position = anticipatedPosition;
        }
    }
}