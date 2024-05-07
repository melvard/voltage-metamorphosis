using System;
using Schemes.Device.Ports;

namespace Schemes.Device.Ports
{
    public class SchemeInteractedOnPortsEventArgs : EventArgs
    {
        public SchemeDevicePortInteractEventArgs schemeDevicePortInteractEventArgs;
        public SchemeDevice schemeDevice;

        public SchemeInteractedOnPortsEventArgs(SchemeDevice schemeDevice,
            SchemeDevicePortInteractEventArgs schemeDevicePortInteractEventArgs)
        {
            this.schemeDevice = schemeDevice;
            this.schemeDevicePortInteractEventArgs = schemeDevicePortInteractEventArgs;
        }
    }
}