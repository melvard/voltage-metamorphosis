using System.Collections.Generic;
using Schemes.Data;
using Schemes.Device.Ports;

namespace Schemes.Device
{
    public struct SchemeDeviceVisualsData
    {
        public SchemeVisualsData schemeVisualsData;
        public List<SchemeDeviceInputPort> schemeDeviceInputPorts;
        public List<SchemeDeviceOutputPort> schemeDeviceOutputPorts;
    }
}