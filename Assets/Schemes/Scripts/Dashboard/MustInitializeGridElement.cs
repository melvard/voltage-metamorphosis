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

        public int X { get; private set; }
        public int Y { get; private set; }
        public SmartGrid<T> Grid { get; }
    }
}