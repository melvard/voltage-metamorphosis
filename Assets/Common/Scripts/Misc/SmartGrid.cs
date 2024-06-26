using System;
using System.Collections;
using System.Collections.Generic;
using Schemes.Data;
using UnityEngine;

namespace Misc
{
    public class SmartGrid<TGridElement> : IEnumerable<TGridElement>
    {
        #region DEBUG_ONLY

        private readonly Color DEBUG_LINE_COLOR = new(136, 8, 8, 0.2f);
        private const float DEBUG_LINE_DURATION = 100000f;

        #endregion

        #region PRIVATE_FIELDS

        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;
        private readonly TGridElement[,] _gridArray;
        private readonly Vector3 _originPosition;
        private readonly bool _showDebug = true;
        
        #endregion

        #region CONSTRUCTORS

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
        }
        
        #endregion

        #region COMMUNICATION_WITH_GRID
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

        public Vector3 GetOrigin()
        {
            return _originPosition;
        }

        public float GetCellSize()
        {
            return _cellSize;
        }

        #endregion

        #region FOREACH_IMPLEMENTATION

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IEnumerator<TGridElement> GetEnumerator()
        {
            foreach (TGridElement element in _gridArray)
            {
                yield return element;
            }
        }

        #endregion
    }
}
//
// [RequireComponent(typeof(MeshFilter))]
// public class GridMesh : MonoBehaviour 
// {
//     public MeshFilter filter;
//     public Vector2Int gridSize;
//     public float gridSpacing;
//
//     Mesh mesh;
//     List<Vector3> verticies;
//     List<int> indices;
//     private bool _isInitialized;
//
//     private void Init(Vector2Int girdSize, float cellSize )
//     {
//         gridSize = girdSize;
//         gridSpacing = cellSize;
//         _isInitialized = true;
//         filter = GetComponent<MeshFilter>();
//         
//         mesh = new Mesh();
//         MeshRenderer meshRenderer = filter.GetComponent<MeshRenderer>();
//         meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
//         meshRenderer.material.color = Color.black;
//
//         Rebuild();
//     }
//
//     private void Update() 
//     {
//         if (_isInitialized)
//         {
//             Rebuild();
//         }
//     }
//
//     private void Rebuild() {
//         verticies = new List<Vector3>();
//         indices = new List<int>();
//
//         float xMin = gridSpacing * gridSize.x / 2f;
//         float zMin = gridSpacing * gridSize.y / 2f;
//
//         for (int i = 0; i <= gridSize.x; i++) {
//             for (int j = 0; j <= gridSize.y; j++) {
//                 float x1 = i * gridSpacing - xMin;
//                 float x2 = (i + 1) * gridSpacing - xMin;
//                 float z1 = j * gridSpacing - zMin;
//                 float z2 = (j + 1) * gridSpacing - zMin;
//
//                 if (i != gridSize.x) {
//                     verticies.Add(new Vector3(x1, 0, z1));
//                     verticies.Add(new Vector3(x2, 0, z1));
//                 }
//
//                 if (j != gridSize.y) {
//                     verticies.Add(new Vector3(x1, 0, z1));
//                     verticies.Add(new Vector3(x1, 0, z2));
//                 }
//             }
//         }
//
//         int indicesCount = verticies.Count;
//         for (int i = 0; i < indicesCount; i++) {
//             indices.Add(i);
//         }
//
//         mesh.vertices = verticies.ToArray();
//         mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
//         filter.mesh = mesh;
//     }
// }