using UnityEngine;

namespace Schemes.Device.Movement
{
    public interface IMovementExecutionStrategy
    {
        void SetAnticipatedPosition(Transform moveable, Vector3 anticipatedPosition);
    }
}