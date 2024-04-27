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
        
        public static List<T1> FindPath<T1>(SmartGrid<T1> grid, int startX, int startY, int endX, int endY) 
            where T1 : class, IGridPathNode<T1>
        {
            T1 startNode = grid.GetValue(startX, startY);
            T1 endNode  = grid.GetValue(endX, endY);

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
                    return CalculatePath(currentNode);
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

        private static List<T> CalculatePath<T>(T node)
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
            return pathNodes;
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