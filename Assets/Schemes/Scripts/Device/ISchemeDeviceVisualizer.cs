using Schemes.Data;

namespace Schemes.Device
{
    public  interface ISchemeDeviceVisualizer
    {
        void Visualise(SchemeDeviceVisualsData schemeDeviceVisualsData);
        // void Visualize(ISchemeDeviceTemplate schemeDeviceTemplate);
    }
}