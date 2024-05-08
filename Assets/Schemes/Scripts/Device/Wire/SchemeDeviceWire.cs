using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Misc;
using Schemes.Dashboard;
using Schemes.Data;
using Schemes.Device.Ports;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Schemes.Device.Wire
{
    public class SchemeDeviceWire : MonoBehaviour
    {
        #region CONSTS

        private const KeyCode CANCEL_WIRING_KEY = KeyCode.Escape;
        private const KeyCode WAYPOINT_GENERATION_KEY = KeyCode.Mouse0;

        #endregion

        #region SERIALIZED_FIELDS

        [AssetsOnly] [SerializeField] private WireBody wireBodyRef;
        [AssetsOnly] [SerializeField] private WireNode wireNodeRef;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private WireValueIndicator wireValueIndicator;
        [SerializeField] private SchemeDeviceWireInteractionController schemeDeviceWireInteractionController;
        
        #endregion

        #region PRIVATE_FIELDS

        private List<WireNode> _wireNodes;
        private CancellationTokenSource _wiringCancellationTokenSource;
        private Vector3[] _lineRendererLocalPositions;
        readonly List<WireNode> _currentWireNodes = new();
        private SchemeDevicePort _startPort;
        private SchemeDevicePort _endPort;
        private int _relationIndex;
        private List<BoxCollider> _interactionColliders;

        #endregion

        #region GETTERS

        public SchemeDevicePort StartPort => _startPort;
        public SchemeDevicePort EndPort => _endPort;

        #endregion

        #region EVENTS

        public event UnityAction OnWiringCanceled;
        public event UnityAction<SchemeDeviceWire> OnWireRemoveCommand;
        
        #endregion

        private WireNode GenerateWaypointNode()
        {
            var node = GenerateNode(null);
            return node;
        }

        //
        // private void GenerateWireBody()
        // {
        //     var wireBody = Instantiate(wireBodyRef, transform, true);
        //     // _wireBodies.Add(wireBody);
        //     // wireBody.SetNode();
        // }

        private WireNode GenerateNode(SchemeDevicePort schemeDevicePort)
        {
            var node = Instantiate(wireNodeRef, transform, true);
            _wireNodes ??= new();
            return node;
        }


        private void Start()
        {
            // todo: maybe this is not the right place for subscribing to interaction controller
            schemeDeviceWireInteractionController.OnRemoveWireClick += () => OnWireRemoveCommand?.Invoke(this); 
        }

        public void StartWiring()
        {
            
            _wiringCancellationTokenSource = new CancellationTokenSource();
            ActiveWiring(_wiringCancellationTokenSource.Token).Forget();
        }

        public void TerminateActiveWiring()
        {
            _wireNodes = _currentWireNodes;
            MarkNodesAsBusy(_wireNodes);
            _interactionColliders = GenerateInteractionColliders(lineRenderer.startWidth, _wireNodes.Select(x=>x.transform).ToList(), transform);
            
            if (_wiringCancellationTokenSource == null) throw new Exception("");
            _wiringCancellationTokenSource.Cancel();
        }

        private List<BoxCollider> GenerateInteractionColliders(float colliderWidth, List<Transform> points, Transform parent)
        {
            List<BoxCollider> colliders = new List<BoxCollider>();

            // Iterate over each pair of points
            for (int i = 0; i < points.Count - 1; i++)
            {
                Transform startPoint = points[i];
                if(i+1 >= points.Count) continue;
                
                Transform endPoint = points[i + 1];

                // Calculate the size and center of the collider
                Vector3 size = new Vector3(colliderWidth, colliderWidth, Vector3.Distance(startPoint.position, endPoint.position));
                Vector3 center = (startPoint.position + endPoint.position) / 2f;

                // Create a new GameObject for the collider
                GameObject colliderObject = new GameObject("Collider" + i);
                colliderObject.transform.SetParent(parent, true);
                colliderObject.transform.position = center;

                // Add a BoxCollider component to the GameObject
                BoxCollider collider = colliderObject.AddComponent<BoxCollider>();

                // Set the size and center of the collider
                collider.size = size;
                // collider.center = center;

                // Rotate the collider to match the direction from startPoint to endPoint
                colliderObject.transform.LookAt(endPoint);

                // Add the collider to the list
                colliders.Add(collider);
            }

            return colliders;
        }


        private void MarkNodesAsBusy(List<WireNode> nodes)
        {
            foreach (var wireNode in nodes)
            {
                wireNode.PathNode.businessIntValDebug = 1;
            }
        }
        
        private async UniTaskVoid ActiveWiring(CancellationToken cancellationToken)
        {
            _currentWireNodes.Clear();
            List<DashboardGridElement> totalPath = new();
            List<Vector3> waypointPositions = new List<Vector3>();
            while (true)
            {
                if (Input.GetKeyDown(CANCEL_WIRING_KEY))
                {
                    CancelWiring();
                    return;
                }

                if (Input.GetKeyDown(WAYPOINT_GENERATION_KEY))
                {
                    // var lastWireNode = _currentWireNodes.Last();
                    var mousePositionToGrid = EditorDashboard.Instance.GetMousePositionToGrid();
                    // bug: if cell position aligns with port, you have a problem baby
                    if (EditorDashboard.Instance.IsGridCellWalkable(mousePositionToGrid))
                    {
                        waypointPositions.Add(mousePositionToGrid);
                    }
                }

                Vector3 startPosition = transform.position;

                totalPath.Clear();
                for (var i = 0; i < waypointPositions.Count; i++)
                {
                    var waypointPosition = waypointPositions[i];

                    var simplifiedPath = EditorDashboard.Instance.GetPathWithWorldPositionsOnGrid(
                        startPosition, waypointPosition, true);
                    if (i != 0 && simplifiedPath != null && simplifiedPath.Count != 0)
                    {
                        simplifiedPath.Pop(0);
                    }

                    if (simplifiedPath != null) totalPath.AddRange(simplifiedPath);
                    startPosition = waypointPosition;
                }

                var lastSimplifiedPath = EditorDashboard.Instance.GetPathWithWorldPositionsOnGrid(startPosition,
                    EditorDashboard.Instance.GetMousePositionToGrid(), true);

                if (!waypointPositions.IsNullOrEmpty() && lastSimplifiedPath != null && lastSimplifiedPath.Count != 0)
                {
                    lastSimplifiedPath.Pop(0);
                }

                if (lastSimplifiedPath != null) totalPath.AddRange(lastSimplifiedPath);

                _currentWireNodes.ForEach(x => Destroy(x.gameObject));
                _currentWireNodes.Clear();

                GenerateWireVisuals(totalPath);

                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.Yield(cancellationToken);
            }
        }


        private void CancelWiring()
        {
            OnWiringCanceled?.Invoke();
            _currentWireNodes.ForEach(x => Destroy(x.gameObject));
            _currentWireNodes.Clear();
            Destroy(gameObject);
        }

        public void SetPosition(Vector3 transformPosition)
        {
            transform.position = transformPosition;
        }

        public void SetStartPort(SchemeDevicePort startPort)
        {
            this._startPort = startPort;
        }

        public void SetEndPort(SchemeDevicePort endPort)
        {
            this._endPort = endPort;
        }

        public void SetRelationIndex(int index)
        {
            this._relationIndex = index;
        }

        public WireConnectionEditorData GetConnectionData()
        {
            var coordinatesOfNodes = _wireNodes
                .Select(wireNode => new Coordinate(wireNode.PathNode.X, wireNode.PathNode.Y)).ToList();
            var wireConnectionEditorData = new WireConnectionEditorData(coordinatesOfNodes, _relationIndex);

            return wireConnectionEditorData;
        }

        public void ConstructWire(WireConnectionEditorData wireConnectionEditorData,
            SchemeDevicePort startPort, SchemeDevicePort endPort)
        {
            SetStartPort(startPort);
            SetEndPort(endPort);
            SetRelationIndex(wireConnectionEditorData.relationIndex);
            GenerateWireVisuals(EditorDashboard.Instance.GetDashboardElementsOnGridWithCoordinates(wireConnectionEditorData.wireNodesCoordinates));
            _wireNodes = _currentWireNodes;
            _interactionColliders = GenerateInteractionColliders(lineRenderer.startWidth, _wireNodes.Select(x=>x.transform).ToList(), transform);
        }

        private void GenerateWireVisuals(List<DashboardGridElement> dashboardGridElements)
        {
            if (dashboardGridElements != null)
            {
                foreach (var dashboardGridElement in dashboardGridElements)
                {
                    var node = GenerateWaypointNode();
                    node.transform.position = dashboardGridElement.GetPositionOnGrid();
                    node.SetPathNode(dashboardGridElement);
                    _currentWireNodes.Add(node);
                }

                var positions = _currentWireNodes.Select(x => x.transform.position);
                var lineRendererPositions = positions.ToArray();
                lineRenderer.positionCount = lineRendererPositions.Length;
                lineRenderer.SetPositions(lineRendererPositions);
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
        }

        public void UpdateWireValue(bool value)
        {
            wireValueIndicator.UpdateWireValue(value);
        }

        public int GetRelationIndex()
        {
            return _relationIndex;
        }

        public void ArrangePorts()
        {
            if (_startPort is not SchemeDeviceOutputPort) (_startPort, _endPort) = (_endPort, _startPort);
        }

        public void DestroyCommand()
        {
            foreach (var currentWireNode in _currentWireNodes)
            {
                currentWireNode.PathNode.businessIntValDebug = 0;
            }

            Destroy(gameObject);
        }
    }
}