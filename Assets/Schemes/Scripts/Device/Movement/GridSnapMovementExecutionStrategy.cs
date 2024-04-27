using UnityEngine;

namespace Schemes.Device.Movement
{
    public class GridSnapMovementExecutionStrategy : IMovementExecutionStrategy
    {
        private IGridHandler _gridHandler;

        public void SetGirdHandler(IGridHandler gridHandler)
        {
            _gridHandler = gridHandler;
        }
        public void SetAnticipatedPosition(Transform moveable, Vector3 anticipatedPosition)
        {
            moveable.position = _gridHandler.GetPositionOnGrid(anticipatedPosition);
        }
    }

    public interface IGridHandler
    {
        Vector3 GetPositionOnGrid(Vector3 position);
    }
}