using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Canvas;
using Cysharp.Threading.Tasks;
using GameLogic;
using Misc;
using Schemes.Data;
using Schemes.Device.Movement;
using UnityEngine;

namespace Schemes.Dashboard
{
    public class EditorDashboard : MonoSingleton<EditorDashboard>, IGridHandler
    {
        #region SERIALIZED_FIELDS

        public Material debugPathMaterial;
        [SerializeField] private Transform ground;
        [SerializeField] private SchemeEditor schemeEditor;
        [SerializeField] private SchemeEditorUI schemeEditorUI;
        [SerializeField] private Transform schemeEditorCameraTransform;
        
        #endregion

        #region PRIVATE_FIELDS

        private SmartGrid<DashboardGridElement> _grid;
        private CancellationTokenSource _saveRemoveSchemeTaskCancellationTokenSource;

        #endregion

        #region GETTERS

        public SchemeEditor SchemeEditor_Debug => schemeEditor;
        
        #endregion
        
        private async void Start()
        {
            _saveRemoveSchemeTaskCancellationTokenSource = new();
            schemeEditorUI.OnSaveSchemeCommandFromUI += OnSaveSchemeHandler;
            schemeEditorUI.OnClearDashboardCommandFromUI += OnClearDashboardHandler;
            schemeEditorUI.OnNewSchemeCommandFromUI += OnNewSchemeHandler;

            Vector3 gridOrigin = transform.position +
                                 new Vector3(-50, ground.position.y + ground.localScale.y + 0.1f, -50);
            _grid = new SmartGrid<DashboardGridElement>(
                200,
                200,
                0.5f,
                gridOrigin,
                (grid, x, y) => new DashboardGridElement(grid, x, y));

            await UniTask.WaitUntil(() => InputsManager.GetKeyDown(KeyCode.Slash, gameObject.layer));
            Debug_GenerateRandomObstacles(_grid);
        }


        private void Debug_GenerateRandomObstacles(SmartGrid<DashboardGridElement> grid)
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

        public void Init()
        {
            schemeEditor.Init();
            schemeEditorUI.Init();
            SetEditorCamCamera(Vector3.zero);
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

        private void OnClearDashboardHandler()
        {
            schemeEditor.ClearComponentsAndWires();
            foreach (var dashboardGridElement in _grid)
            {
                dashboardGridElement.businessIntValDebug = 0;
            }
        }
        
        public void OnSchemeSelectCommandHandler(SchemeInteractionEventArgs arg0)
        {
            schemeEditor.GenerateDevice(arg0.scheme);
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
            var schemeToSave =  schemeEditor.PreapareSchemeForSave();
            schemeToSave.SchemeData.UpdateDataFromUI(schemeUIData);
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

        private void SetEditorCamCamera(Vector3 position)
        {
            schemeEditorCameraTransform.position = position;
        }

        public void OnRemoveSchemeHandler(SchemeInteractionEventArgs arg0)
        {
            SchemesSaverLoader.OnRemoveSchemeHandler(arg0, _saveRemoveSchemeTaskCancellationTokenSource.Token);
        }
    }

    public class DashboardGridElement : MustInitializeGridElement<DashboardGridElement>,
        IGridPathNode<DashboardGridElement>
    {
        public DashboardGridElement(SmartGrid<DashboardGridElement> grid, int x, int y) : base(grid, x, y)
        {
        }

        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost { get; set; }

        public int businessIntValDebug = 0;
        public DashboardGridElement nodeCameFrom { get; set; }

        public bool IsWalkable => businessIntValDebug == 0;
    }
}