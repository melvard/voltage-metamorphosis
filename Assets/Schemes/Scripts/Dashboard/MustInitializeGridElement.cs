using Misc;
using UnityEngine;

namespace Schemes.Dashboard
{
    public abstract class MustInitializeGridElement<T> : MustInitialize<SmartGrid<T>, int, int>, IGridElement<T> 
        where T : IGridElement<T>
    {
        protected MustInitializeGridElement(SmartGrid<T> grid, int x, int y) : base(grid, x, y)
        {
            X = x;
            Y = y;
            Grid = grid;
            // Debug.Log(Grid.GetHeight());
        }

        public int X { get; }
        public int Y { get; }
        public SmartGrid<T> Grid { get; }
        public Vector3 GetPositionOnGrid()
        {
            return Grid.GetWorldPosition(X, Y);
        }
    }
}