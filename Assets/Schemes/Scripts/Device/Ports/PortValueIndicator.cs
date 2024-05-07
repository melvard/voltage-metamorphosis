using Schemes.Dashboard;
using Schemes.LogicUnit;
using UnityEngine;

namespace Schemes.Device.Ports
{
    public class PortValueIndicator : MonoBehaviour
    {
        [SerializeField] private Material enabledMaterial;
        [SerializeField] private Material disabledMaterial;
        [SerializeField] private MeshRenderer portMeshRenderer;
        [SerializeField] private bool isInput;
        
        private int _portIndex;
        private int _deviceIndex;
        private bool _value;
        
        public void Init(int deviceIndex, int portIndex)
        {
            _portIndex = portIndex;
            _deviceIndex = deviceIndex;
        }
        public void UpdatePortValue(bool portValue)
        {
            _value = portValue;
            portMeshRenderer.sharedMaterial = _value ? enabledMaterial : disabledMaterial;
        }
    }
}