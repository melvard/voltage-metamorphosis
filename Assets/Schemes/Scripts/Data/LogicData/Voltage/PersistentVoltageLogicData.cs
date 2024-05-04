using System;
using UnityEngine;

namespace Schemes.Data.LogicData.Voltage
{
    [Serializable]
    public class PersistentVoltageLogicData : SchemeLogicData, IOutputPortSchemeLogicData
    {
        [field:SerializeField] public byte NumberOfOutputs { get; set;  }
        // public override byte GetVoltage(int portIndex)
        // {
        //     return 1;
        // }
    }
}