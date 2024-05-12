// using Schemes.Dashboard;
//
// namespace Misc
// {
//     public class PathNode : IGridElement
//     {
//         private SmartGrid<PathNode> _grid;
//
//         public int gCost;
//         public int hCost;
//         public int fCost;
//
//         public PathNode nodeCameFrom;
//
//         public int X { get; }
//
//         public int Y { get; }
//         // public SmartGrid<IGridElement> Grid { get; }
//
//         public PathNode(SmartGrid<PathNode> grid, int x, int y)
//         {
//             _grid = grid;
//             X = x;
//             Y = y;
//         }
//
//         public void CalculateFCost()
//         {
//             fCost = gCost + hCost;
//         }
//     }
// }