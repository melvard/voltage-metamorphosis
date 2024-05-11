using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Schemes.Dashboard;
using UnityEngine;

namespace Misc
{
    public static class AStarPathfinding
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;
        
        // private SmartGrid<T> _grid;
        
        public static List<T1> FindPath<T1>(SmartGrid<T1> grid, int startX, int startY, int endX, int endY, bool simplified) 
            where T1 : class, IGridPathNode<T1>
        {
            T1 startNode = grid.GetValue(startX, startY);
            T1 endNode  = grid.GetValue(endX, endY);

            if (!endNode.IsWalkable) return null;
            
            var openList = new List<T1>();
            var closeList = new List<T1>();
            openList.Add(startNode);

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    T1 pathNode = grid.GetValue(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.nodeCameFrom = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);

            while (openList.Count > 0)
            {
                T1 currentNode = GetLowestCostNode(openList);
                if (currentNode.Equals(endNode))
                {
                    return CalculatePath(currentNode, simplified);
                }

                openList.Remove(currentNode);
                closeList.Add(currentNode);
                List<T1> nodeNeighbours = GetNodeNeighbours(grid, currentNode);
                foreach (var neighbour in nodeNeighbours)
                {
                    if(closeList.Contains(neighbour)) continue;
                    if (!neighbour.IsWalkable)
                    {
                        closeList.Add(neighbour);
                        continue;
                    }

                    var tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbour);
                    if (tentativeGCost < neighbour.gCost)
                    {
                        neighbour.nodeCameFrom = currentNode;
                        neighbour.gCost = tentativeGCost;
                        neighbour.hCost = CalculateDistanceCost(neighbour, endNode);
                        neighbour.CalculateFCost();

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                } 
            }
            
            return null;
        }

        private static List<T> GetNodeNeighbours<T>(SmartGrid<T> grid,  T node)
            where T : class, IGridPathNode<T>
        {
            var neighbourNodes = new List<T>();

            if (node.X - 1 >= 0)
            {
                //left
                neighbourNodes.Add(grid.GetValue(node.X - 1, node.Y));
                //left down
                if(node.Y - 1 >= 0) neighbourNodes.Add(grid.GetValue(node.X-1, node.Y - 1));
                //left up
                if(node.Y + 1 <  grid.GetHeight()) neighbourNodes.Add(grid.GetValue(node.X-1, node.Y + 1));
            }
            if(node.X + 1 < grid.GetWidth())
            {
                //right
                neighbourNodes.Add(grid.GetValue(node.X + 1, node.Y));
                //right down
                if(node.Y - 1 >= 0) neighbourNodes.Add(grid.GetValue(node.X+1, node.Y - 1));
                //right up
                if(node.Y + 1 < grid.GetHeight()) neighbourNodes.Add(grid.GetValue(node.X+1, node.Y + 1));
            }

            // up
            if (node.Y + 1 < grid.GetHeight())
            {
                neighbourNodes.Add(grid.GetValue(node.X, node.Y + 1));
            }
            //down
            if (node.Y - 1 >= 0)
            {
                neighbourNodes.Add(grid.GetValue(node.X, node.Y - 1));
            }

            return neighbourNodes;
        }

        private static List<T> CalculatePath<T>(T node, bool simplified)
            where T : class, IGridPathNode<T>
        {
            List<T> pathNodes = new List<T>();
            pathNodes.Add(node);
            T currentNode = node;
            while (currentNode.nodeCameFrom != null)
            {
                currentNode = currentNode.nodeCameFrom;
                pathNodes.Add(currentNode);
            }
            pathNodes.Reverse();
            if (simplified)
            {
                pathNodes = SimplifyPath(pathNodes);
            }
            return pathNodes;
            
        }

        private static List<T> SimplifyPath<T>(List<T> path) where T : class, IGridPathNode<T>
        {
            List<T> simplifiedPath = new();

            if (path.Count == 0)
                return simplifiedPath;

            // Add the start coordinate
            simplifiedPath.Add(path[0]);

            // Iterate through the path to find turns and end coordinate
            for (int i = 1; i < path.Count - 1; i++)
            {
                // Check if the current point forms a turn with the previous and next points
                if (IsTurn(path[i - 1], path[i], path[i + 1]))
                {
                    simplifiedPath.Add(path[i]); // Add the turn
                }
            }

            // Add the end coordinate
            simplifiedPath.Add(path[path.Count - 1]);

            return simplifiedPath;
        }
        
        static bool IsTurn<T>(T prev, T current, T next) where T : class, IGridPathNode<T>
        {
            // Check if the current point forms a turn with the previous and next points
            int dx1 = current.X - prev.X;
            int dy1 = current.Y - prev.Y;
            int dx2 = next.X - current.X;
            int dy2 = next.Y - current.Y;

            return dx1 * dy2 != dx2 * dy1;
        }
        
        private static T GetLowestCostNode<T>(List<T> listOfNodes)
            where T : class, IGridPathNode<T>
        {
            T lowestCostNode = listOfNodes[0];
            for (int i = 0; i < listOfNodes.Count; i++)
            {
                if (listOfNodes[i].fCost < lowestCostNode.fCost)
                {
                    lowestCostNode = listOfNodes[i];
                }
            }

            return lowestCostNode;
        }

        private static int CalculateDistanceCost<T>(IGridElement<T> a, IGridElement<T> b)
        {
            int xDistance = Mathf.Abs(a.X - b.X);
            int yDistance = Mathf.Abs(a.Y - b.Y);
            int remaining = Mathf.Abs(xDistance - yDistance);

            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;

        }
        
    }
}