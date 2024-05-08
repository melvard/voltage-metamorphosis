using System;
using System.Collections;
using System.Collections.Generic;
using Schemes.Data;
using UnityEngine;

namespace Misc
{
    public class SmartGrid<TGridElement> : IEnumerable<TGridElement>
    {
        private readonly Color DEBUG_LINE_COLOR = new(136, 8, 8, 0.2f);
        private const float DEBUG_LINE_DURATION = 100000f;

        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;
        private readonly TGridElement[,] _gridArray;
        private readonly Vector3 _originPosition;
        private TextMesh[,] _debugTextsGrid;

        private bool _showDebug = true;
        public SmartGrid(int width, int height, float cellSize, Vector3 originPosition, Func<SmartGrid<TGridElement>, int, int, TGridElement> createGridObject)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;

            _gridArray = new TGridElement[width, height];
            for (int x = 0; x < _gridArray.GetLength(0); x++)
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _gridArray[x, y] = createGridObject(this, x, y);
                }

            if (_showDebug)
            {
                Vector3 shiftFromCellCenter = new Vector3(-cellSize / 2f, 0, -cellSize / 2f);
                _debugTextsGrid = new TextMesh[width, height];
                for (int x = 0; x < _gridArray.GetLength(0); x++)
                    for (int y = 0; y < _gridArray.GetLength(1); y++)
                    {
                        // _debugTextsGrid[x, y] = Utilities.CreateWorldText(_gridArray[x,y].ToString(), null, GetWorldPosition(x,y));
                        Debug.DrawLine(
                            GetWorldPosition(x, y) + shiftFromCellCenter,
                            GetWorldPosition(x, y + 1) + shiftFromCellCenter,
                            DEBUG_LINE_COLOR,
                            DEBUG_LINE_DURATION);
                        Debug.DrawLine(
                            GetWorldPosition(x, y) + shiftFromCellCenter ,
                            GetWorldPosition(x + 1, y) + shiftFromCellCenter, 
                            DEBUG_LINE_COLOR,
                            DEBUG_LINE_DURATION);
                    }
                Debug.DrawLine(
                    GetWorldPosition(0, height) + shiftFromCellCenter, 
                    GetWorldPosition(width, height) + shiftFromCellCenter,
                    DEBUG_LINE_COLOR,
                    DEBUG_LINE_DURATION);
                Debug.DrawLine(
                    GetWorldPosition(width, 0) + shiftFromCellCenter, 
                    GetWorldPosition(width, height) + shiftFromCellCenter, 
                    DEBUG_LINE_COLOR,
                    DEBUG_LINE_DURATION);
            }

            // GenerateRandomObstacles();
        }

        // public SmartGrid(Vector2Int size, float cellSize, Vector3 originPosition, Func<TGridObject> createGridObject) : 
        //     this(size.x, size.y, cellSize,
        //     originPosition, createGridObject)
        // {
        // }

        public void SetValue(int x, int y, TGridElement value)
        {
            if
                (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _gridArray[x, y] = value;
            }
        }

        public void SetValue(Coordinate coordinate, TGridElement value)
        {
            var x = coordinate.x;
            var y = coordinate.y;
            SetValue(coordinate.x, coordinate.y, value);
        }
        
        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.RoundToInt((worldPosition.x - _originPosition.x) / _cellSize);
            y = Mathf.RoundToInt((worldPosition.z - _originPosition.z) / _cellSize);

            x = Mathf.Clamp(x, 0, _width-1);
            y = Math.Clamp(y, 0, _height - 1);
        }
        public Coordinate GetXY(Vector3 worldPosition)
        {
            GetXY(worldPosition, out var x, out var y);
            return new Coordinate(x, y);
        }

        public Vector3 GetAlignedPositionOnGrid(Vector3 worldPosition)
        {
            GetXY(worldPosition, out int x, out int y);
            var worldPositionOnGrid = GetWorldPosition(x, y);

            // #if UNITY_EDITOR
            // var go1 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            // go1.transform.localScale = Vector3.one * 0.5f;
            // go1.transform.position = worldPosition;
            //
            // var go2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // go2.transform.localScale = Vector3.one * 0.2f;
            // go2.transform.position = worldPositionOnGrid;
            // #endif

            return worldPositionOnGrid;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, 0, y) * _cellSize + _originPosition;
        }
        
        public Vector3 GetWorldPosition(Coordinate coordinate)
        {
            return GetWorldPosition(coordinate.x, coordinate.y);
        }

        public void SetValue(Vector3 worldPosition, TGridElement value)
        {
            GetXY(worldPosition, out int x, out int y);
            SetValue(x, y, value);
        }

        public TGridElement GetValue(int x, int y)
        {
            if(x >= 0 && y >=0 && x < _width && y < _height)
                return _gridArray[x,y];
            return default;
        }
        
        public TGridElement GetValue(Coordinate coordinate)
        {
            return GetValue(coordinate.x, coordinate.y);
        }

        public TGridElement GetValue(Vector3 worldPosition)
        {
            GetXY(worldPosition, out int x, out int y);
            return GetValue(x, y);
        }

        public int GetWidth()
        {
            return _width;
        }
        
        public int GetHeight()
        {
            return _height;
        }

        public IEnumerator<TGridElement> GetEnumerator()
        {
            foreach (TGridElement element in _gridArray)
            {
                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}