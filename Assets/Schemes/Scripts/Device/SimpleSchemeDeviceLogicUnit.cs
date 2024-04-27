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

    // public struct SchemeLogicInstance
    // {
    //     public struct InputPortInfo
    //     {
    //         public bool isPortVoltageDetermined;
    //         public byte voltage;
    //     }
    //     private SchemeLogicData _schemeLogicData;
    //
    //     // public InputPortInfo[] outputs;
    //     private InputPortInfo[] _inputPortInfos;
    //
    //     public SchemeLogicInstance(SchemeLogicData schemeLogicData)
    //     {
    //        
    //         _schemeLogicData = schemeLogicData;
    //     }
    //
    //     public byte[] GetOutputs()
    //     {
    //         for (int i = 0; i < _inputPortInfos.Length; i++)
    //         {
    //             _inputPortInfos[i]
    //         }
    //     }
    //     
    // }
}