using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Misc;
using Schemes.Dashboard;
using Schemes.Device.Movement;
using Schemes.Device.Ports;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Schemes.Device.Wire
{
    public class SchemeDeviceWire : MonoBehaviour
    {
        [AssetsOnly] [SerializeField] private WireBody wireBodyRef;
        [AssetsOnly] [SerializeField] private WireNode wireNodeRef;
        [SerializeField] private LineRenderer lineRenderer;
        
        // private SchemeDevicePort[] _connectingPorts;
        private List<WireNode> _wireNodes;
        // private List<WireBody> _wireBodies;

        private CancellationTokenSource _wiringCancellationTokenSource;
        private Vector3[] _lineRendererLocalPositions;
        readonly List<WireNode> _currentWireNodes = new();
        private SchemeDevicePort _startPort;
        private SchemeDevicePort _endPort;

        public SchemeDevicePort StartPort => _startPort;
        public SchemeDevicePort EndPort => _endPort;

        // private IGridHandler _gridHandler;
        public void Init()
        {
            // _numberOfPositions = 2;
            // _wireBodies = new();
            // _lineRendererLocalPositions = new Vector3[2];
            // lineRenderer.SetPositions(_lineRendererLocalPositions);
        }

        private void UpdateLineRenderer()
        {
        }

        private void UpdateLineRendererLastPosition(Vector3 localPosition)
        {
        }
        // private WireNode FirstNode => _wireNodes[0];
        // private WireNode LastNode => _wireNodes[1];

        // public SchemeDevicePort this[int index]
        // {
        //     get
        //     {
        //         ValidateIndex(index);
        //         return _connectingPorts[index];
        //
        //     }
        //     set
        //     {
        //         ValidateIndex(index);
        //         _connectingPorts[index] = value;
        //         GenerateNode(value);
        //     }
        // }

        // public void GenerateFirstNode(SchemeDevicePort schemeDevicePort)
        // {
        //     var node = GenerateNode(schemeDevicePort);
        //     _wireNodes.Add(node);
        // }
        //
        // public void GenerateLastNode(SchemeDevicePort schemeDevicePort)
        // {
        //     var node = GenerateNode(schemeDevicePort);
        //     _wireNodes.Add(node);
        // }

        public WireNode GenerateWaypointNode()
        {
            var node = GenerateNode(null);
            return node;
        }

        private void GenerateWireBody()
        {
            var wireBody = Instantiate(wireBodyRef, transform, true);
            // _wireBodies.Add(wireBody);
            // wireBody.SetNode();
        }

        private WireNode GenerateNode(SchemeDevicePort schemeDevicePort)
        {
            var node = Instantiate(wireNodeRef, transform, true);
            _wireNodes ??= new();
            return node;
        }


        public void StartWiring()
        {
            
            _wiringCancellationTokenSource = new CancellationTokenSource();
            ActiveWiring(_wiringCancellationTokenSource.Token).Forget();
        }
        
        public void StopWiring()
        {
            if (_wiringCancellationTokenSource == null) throw new Exception("");
            _wiringCancellationTokenSource.Cancel();
        }

       

        private async UniTaskVoid ActiveWiring(CancellationToken cancellationToken)
        {
            _currentWireNodes.Clear();
            while (true)
            {
                
                // var currentWireBody = _wireBodies.Last();
                // var endWireNode = _wireNodes[^1];
                var simplifiedPath = EditorDashboard.Instance.GetPathWithWorldPositionsOnGrid(
                    transform.position, EditorDashboard.Instance.GetPositionOnGridWithMouse(), true);
                
                _currentWireNodes.ForEach(x=>Destroy(x.gameObject));
                _currentWireNodes.Clear();

                if (simplifiedPath != null)
                {
                    foreach (var dashboardGridElement in simplifiedPath)
                    {
                        var node = GenerateWaypointNode();
                        node.transform.position = dashboardGridElement.GetPositionOnGrid();
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
                    // lineRenderer.SetPositions(null);
                }

                // endWireNode.transform.position = EditorDashboard.Instance.GetPositionOnGridWithMouse();
                // var diff = endWireNode.transform.position - startWireNode.transform.position;
                //
                // Transform wireTransform = currentWireBody.transform;
                //
                // wireTransform.rotation = Quaternion.LookRotation(diff.normalized);
                // var bodyLength = diff.magnitude;
                // var scale= wireTransform.localScale;
                // scale.z = bodyLength;
                // wireTransform.localScale = scale;
                
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.Yield(cancellationToken);
            }
        }


        //
        // private void ValidateIndex(int index)
        // {
        //     if (index < 0 || index >= _connectingPorts.Length)
        //     {
        //         throw new IndexOutOfRangeException();
        //     }
        // }
        public void SetPosition(Vector3 transformPosition)
        {
            transform.position = transformPosition;
        }

        // public void SetGridHandler(IGridHandler gridHandler)
        // {
        //     this._gridHandler = gridHandler;
        // }
        public void SetStartPort(SchemeDevicePort startPort)
        {
            this._startPort = startPort;

        }
        public void SetEndPort(SchemeDevicePort endPort)
        {
            this._endPort = endPort;
        }
        
    }
}