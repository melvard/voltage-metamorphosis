using System;
using Cysharp.Threading.Tasks;
using Misc;
using Schemes.Device.Movement;
using UnityEngine;

namespace Schemes.Dashboard
{
    public class EditorDashboard : MonoSingleton<EditorDashboard>, IGridHandler
    {
        public Material debugPathMaterial;
        [SerializeField] private Transform ground;
        private SmartGrid<DashboardGridElement> _grid;
    
        private async void Start()
        { 
            Vector3 gridOrigin = transform.position + new Vector3(-50,ground.position.y + ground.localScale.y + 0.1f, -50);
            _grid = new SmartGrid<DashboardGridElement>(
                200, 
                200, 
                0.5f, 
                gridOrigin, 
                (grid, x, y) => new DashboardGridElement(grid, x, y));

            await UniTask.WaitUntil(()=> Input.GetKeyDown(KeyCode.C));
            Debug_GenerateRandomObstacles(_grid);
            await UniTask.WaitUntil(()=> Input.GetKeyDown(KeyCode.C));
            
            var path = AStarPathfinding.FindPath(_grid, 0, 0, 199, 199);
            foreach (var gridPathNode in path)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.localScale = Vector3.one * 0.5f;
                go.GetComponent<MeshRenderer>().material = debugPathMaterial;
                go.transform.position = _grid.GetWorldPosition(gridPathNode.X, gridPathNode.Y);
            } 
        }
        
        private void Debug_GenerateRandomObstacles(SmartGrid<DashboardGridElement> grid)
        {
            UnityEngine.Random.InitState(1);
            for (int x = 0; x < grid.GetWidth(); x++)
            for (int y = 0; y <  grid.GetHeight(); y++)
            {
                if (UnityEngine.Random.value > 0.9f)
                {
                    var cell = grid.GetValue(x,y);
                    cell.businessIntValDebug = 1;
                    var go1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go1.transform.localScale = Vector3.one * 0.5f;
                    go1.transform.position = _grid.GetWorldPosition(x,y);
                }
            }
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

        }
        

        public Vector3 GetPositionOnGrid(Vector3 position)
        {
            return _grid.GetAlignedPositionOnGrid(position);
        }
    }

    public class DashboardGridElement : MustInitializeGridElement<DashboardGridElement>, IGridPathNode<DashboardGridElement>
    {
        public DashboardGridElement(SmartGrid<DashboardGridElement> grid, int x, int y) : base(grid, x, y) {}
        
        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost { get; set; }

        public int businessIntValDebug = 0;
        public DashboardGridElement nodeCameFrom { get; set; }
        
        public bool IsWalkable =>  businessIntValDebug == 0;
    }
}


