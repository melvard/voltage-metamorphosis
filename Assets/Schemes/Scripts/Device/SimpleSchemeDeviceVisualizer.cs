using System.Collections.Generic;
using Schemes.Data;
using Schemes.Device.Ports;
using TMPro;
using UnityEngine;

namespace Schemes.Device
{
    public class SimpleSchemeDeviceVisualizer : MonoBehaviour,  ISchemeDeviceVisualizer
    {
        [SerializeField] private TextMeshPro displayName;
        [SerializeField] private Transform deviceBody;
        [SerializeField] private Transform portsContainer;

        private SchemeDeviceVisualsData _schemeSchemeVisualsData;

        public void Visualise(SchemeDeviceVisualsData schemeDeviceVisualsData)
        {
            _schemeSchemeVisualsData = schemeDeviceVisualsData;
            SetDisplayText(_schemeSchemeVisualsData.schemeVisualsData.DisplayName);
            SetSize(schemeDeviceVisualsData.schemeVisualsData.Size);
            ArrangePortsPositions();
        }

        private void ArrangePortsPositions()
        {
            var schemeDeviceInputPorts = _schemeSchemeVisualsData.schemeDeviceInputPorts;
            var schemeDeviceOutputPorts = _schemeSchemeVisualsData.schemeDeviceOutputPorts;
            
            for (int i = 0; i < schemeDeviceInputPorts.Count; i++)
            {
                var position = _schemeSchemeVisualsData.schemeVisualsData.GetInputPortPosition(i);
                // todo: handle positioning in order not to depend the implementer 
                PositionPort(schemeDeviceInputPorts[i], position);
            }
            
            for (int i = 0; i < schemeDeviceOutputPorts.Count; i++)
            {
                var position = _schemeSchemeVisualsData.schemeVisualsData.GetOutputPortPosition(i);
                PositionPort(schemeDeviceOutputPorts[i], position);
            }
        }

        private void PositionPort(SchemeDevicePort schemeDevicePort, Vector2 position)
        {
            schemeDevicePort.transform.SetParent(portsContainer);
            schemeDevicePort.transform.localPosition = new Vector3(position.x, 0, position.y);
        }
        
        private void SetDisplayText(string displayText)
        {
            displayName.text = displayText;
        }
        
        private void SetSize(Vector2 bodySize)
        {
            Vector3 deviceBodyScale = deviceBody.localScale;
            deviceBodyScale.x *= bodySize.x;
            deviceBodyScale.z *= bodySize.y;
            deviceBody.localScale = deviceBodyScale;
        }
    }
}