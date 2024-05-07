using UnityEngine;

namespace Schemes.Device.Wire
{
    public class WireValueIndicator : MonoBehaviour
    {
        [SerializeField] private Material enabledWireBodyMaterial; 
        [SerializeField] private Material disabledWireBodyMaterial;
        [SerializeField] private LineRenderer lineRenderer;
        
        public void UpdateWireValue(bool value)
        {
            lineRenderer.sharedMaterial = value ? enabledWireBodyMaterial : disabledWireBodyMaterial;
        }
    }
}