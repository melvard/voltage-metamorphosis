using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Canvas;
using Common.Misc;
using Cysharp.Threading.Tasks;
using GameLogic;
using Misc;
using Schemes.Data;
using Schemes.Device.Movement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Schemes.Dashboard
{
    public class EditorDashboard : MonoSingleton<EditorDashboard>, IGridHandler
    {
        #region SERIALIZED_FIELDS

        [SerializeField] private Transform ground;
        [SerializeField] private SchemeEditor schemeEditor;
        [SerializeField] private SchemeEditorUI schemeEditorUI;
        [SerializeField] private Transform schemeEditorCameraTransform;

        [Title("Grid settings")] 
        [SerializeField] private Material girdDashboardMaterial;
        [SerializeField] private int gridWidth = 200;
        [SerializeField] private int gridHeight = 200;
        [SerializeField] private float gridCellSize = 0.5f;
        
        #endregion

        #region PRIVATE_FIELDS

        private SmartGrid<DashboardGridElement> _grid;
        private CancellationTokenSource _saveRemoveSchemeTaskCancellationTokenSource;

        #endregion

        #region GETTERS

        public SchemeEditor SchemeEditor_Debug => schemeEditor;
        
        #endregion

        #region UNITY_EVENT_FUNCTIONS

        private async void Start()
        {
            _saveRemoveSchemeTaskCancellationTokenSource = new();
            schemeEditorUI.OnSaveSchemeCommandFromUI += OnSaveSchemeHandler;
            schemeEditorUI.OnClearDashboardCommandFromUI += OnClearDashboardHandler;
            schemeEditorUI.OnNewSchemeCommandFromUI += OnNewSchemeHandler;

            SetupGridSystem();
            
            await UniTask.WaitUntil(() => InputsManager.GetKeyDown(KeyCode.Slash, gameObject.layer));
            Debug_GenerateRandomObstaclesOnGrid(_grid);
        }
        

        #endregion

        #region GRID_SYSTEM
        
        private void SetupGridSystem()
        {
            var offsetX = gridWidth / 2f * gridCellSize;
            var offsetZ = gridHeight / 2f * gridCellSize;

            Vector3 gridOrigin = transform.position +
                                 new Vector3(-offsetX, ground.position.y + ground.localScale.y + 0.1f, -offsetZ);
            var groundScale = ground.transform.localScale;
            groundScale.x = gridWidth * gridCellSize;
            groundScale.z = gridHeight * gridCellSize;

            ground.transform.localScale = groundScale;
            girdDashboardMaterial.mainTextureScale = new Vector2(gridWidth * gridCellSize, gridHeight * gridCellSize);

            _grid = new SmartGrid<DashboardGridElement>(
                gridWidth,
                gridHeight,
                gridCellSize,
                gridOrigin,
                (grid, x, y) => new DashboardGridElement(grid, x, y));
        }

        private void Debug_GenerateRandomObstaclesOnGrid(SmartGrid<DashboardGridElement> grid)
        {
            UnityEngine.Random.InitState(1);
            for (int x = 0; x < grid.GetWidth(); x++)
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                if (UnityEngine.Random.value > 0.9f)
                {
                    var cell = grid.GetValue(x, y);
                    cell.businessIntValDebug = 1;
                    var go1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go1.transform.localScale = Vector3.one * 0.5f;
                    go1.transform.position = _grid.GetWorldPosition(x, y);
                }
            }

            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        }
        
        
        public Vector3 GetPositionOnGrid(Vector3 position)
        {
            return _grid.GetAlignedPositionOnGrid(position);
        }

        public Vector3 GetPositionOnGrid(Coordinate coordinate)
        {
            return _grid.GetWorldPosition(coordinate);
        }

        public Vector3 GetMousePositionToGrid()
        {
            Plane plane = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            plane.Raycast(ray, out var distance);
            return GetPositionOnGrid(ray.GetPoint(distance));
        }

        public List<DashboardGridElement> GetPathWithWorldPositionsOnGrid(Vector3 pos1, Vector3 pos2, bool simplified)
        {
            _grid.GetXY(pos1, out var startX, out var startY);
            _grid.GetXY(pos2, out var endX, out var endY);
            return AStarPathfinding.FindPath(_grid, startX, startY, endX, endY, simplified);
        }
        
        public bool IsGridCellWalkable(Vector3 mousePositionToGrid)
        {
            return _grid.GetValue(mousePositionToGrid).IsWalkable;
        }

        public DashboardGridElement GetDashboardElementOnGrid(Vector3 position)
        {
            return _grid.GetValue(position);
        }

        public List<DashboardGridElement> GetDashboardElementsOnGridWithCoordinates(
            List<Coordinate> wireNodesCoordinates)
        {
            return wireNodesCoordinates.Select(wireNodesCoordinate => _grid.GetValue(wireNodesCoordinate)).ToList();
        }

        #endregion

        #region INITALIZATION

        public void Init()
        {
            schemeEditor.Init();
            schemeEditorUI.Init();
            SetEditorCamCamera(Vector3.zero);
        }
        
        #endregion
        
        #region EVENT_OBSERVATION

        private void OnClearDashboardHandler()
        {
            schemeEditor.ClearComponentsAndWires();
            foreach (var dashboardGridElement in _grid)
            {
                dashboardGridElement.businessIntValDebug = 0;
            }
        }

        public async void OnSchemeSelectCommandHandler(SchemeInteractionEventArgs arg0)
        {
            schemeEditorUI.DisableSchemeSelector();
            await schemeEditor.GenerateDevice(arg0.scheme);
            schemeEditorUI.EnableSchemeSelector();
        }

        public void OnSchemeEditHandler(SchemeInteractionEventArgs arg0)
        {
            if(!arg0.scheme.SchemeData.IsEditable) return;
            schemeEditor.ResetEditor();
            schemeEditor.LoadSchemeInEditor(arg0.scheme);
            SetEditorCamCamera(_grid.GetWorldPosition(arg0.scheme.SchemeData.SchemeEditorData.cameraPositionOnGrid));
        }
        
        private async void OnSaveSchemeHandler(SchemeUIData schemeUIData)
        {
            var schemeToSave =  schemeEditor.PreapareSchemeForSave(schemeUIData);
            schemeToSave.SchemeData.SchemeEditorData.cameraPositionOnGrid = _grid.GetXY(schemeEditorCameraTransform.position);
            schemeToSave.SchemeData.SchemeVisualsData.PendingForTextureCapture = true;
            await SchemesSaverLoader.SaveScheme(schemeToSave, _saveRemoveSchemeTaskCancellationTokenSource.Token);
        }
        
        private void OnNewSchemeHandler()
        {
            var newScheme = schemeEditor.NewScheme();
            SetEditorCamCamera(_grid.GetWorldPosition(_grid.GetHeight()/2, _grid.GetWidth()/2));
            foreach (var dashboardGridElement in _grid)
            {
                dashboardGridElement.businessIntValDebug = 0;
            }
            
        }

        public void OnRemoveSchemeHandler(SchemeInteractionEventArgs arg0)
        {
            SchemesSaverLoader.OnRemoveSchemeHandler(arg0, _saveRemoveSchemeTaskCancellationTokenSource.Token);
        }
        
        private void SetEditorCamCamera(Vector3 position)
        {
            schemeEditorCameraTransform.position = position;
        }
        
        #endregion

    }

    public class DashboardGridElement : MustInitializeGridElement<DashboardGridElement>,
        IGridPathNode<DashboardGridElement>
    {
        public DashboardGridElement(SmartGrid<DashboardGridElement> grid, int x, int y) : base(grid, x, y)
        {
        }

        #region PATHFINDING_PROPERTIES

        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost { get; set; }
        
        public DashboardGridElement nodeCameFrom { get; set; }
        
        public bool IsWalkable => businessIntValDebug == 0;

        #endregion

        #region DEBUG_ONLY

        public int businessIntValDebug = 0;
        
        #endregion
        

    }
}