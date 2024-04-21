using Schemes.Data;
using UnityEngine;

namespace Schemes.Device
{
    public class SimpleSchemeDeviceLogicUnit : ISchemeLogicUnit
    {
        private SchemeLogicData _schemeLogicData;
        
        public void Logigalize(SchemeLogicData schemeLogicData)
        {
            _schemeLogicData = schemeLogicData;
        }
    }
}