using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Schemes.Device.Ports;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Schemes.Device.Wire
{
    public class SchemeDeviceWire : MonoBehaviour
    {
        [AssetsOnly] [SerializeField] private WireBody wireBodyRef;
        [AssetsOnly][SerializeField] private WireNode wireNodeRef;
        [SerializeField] private LineRenderer lineRenderer;

        [PropertySpace]
        // [ChildGameObjectsOnly][SerializeField] private Transform container;
        
        // private SchemeDevicePort[] _connectingPorts;
        private List<WireNode> _wireNodes;
        private List<WireBody> _wireBodies;
        
        private CancellationTokenSource _wiringCancellationTokenSource;
        private Vector3[] _lineRendererLocalPositions;
        
        public void Init()
        {
            // _numberOfPositions = 2;
            _lineRendererLocalPositions = new Vector3[2];
            lineRenderer.SetPositions(_lineRendererLocalPositions);
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

        public void GenerateFirstNode(SchemeDevicePort schemeDevicePort)
        {
            var node = GenerateNode(schemeDevicePort);
            _wireNodes.Add(node);
        }

        public void GenerateLastNode(SchemeDevicePort schemeDevicePort)
        {
            var node = GenerateNode(schemeDevicePort);
            _wireNodes.Add(node);
        }

        public void GenerateWaypoint()
        {
            var node = GenerateNode(null);
            _wireNodes.Add(node);
        }

        private void GenerateWireBody()
        {
            var wireBody = Instantiate(wireBodyRef, transform, true);
            _wireBodies.Add(wireBody);
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
            while (true)
            {
                
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
    }

    public class WireBody : MonoBehaviour
    {
        
    }
}