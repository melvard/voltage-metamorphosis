using UnityEngine;

namespace Schemes.Device.Movement
{
    public interface IMoveable
    {
        void SetPosition(Vector3 position);
        Vector3 GetPosition();
    }
}