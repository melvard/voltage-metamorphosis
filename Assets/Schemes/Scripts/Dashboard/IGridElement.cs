using Misc;

namespace Schemes.Dashboard
{
    public interface IGridElement<T>
    {
        int X { get; }
        int Y { get; }

        SmartGrid<T> Grid { get; }
        // static T GetBusyElement() => throw new NotImplementedException();
    }

    //Note : notice that grid node is limited to A* path finding algorithm and need more generalization if other algorithms are to be used
}