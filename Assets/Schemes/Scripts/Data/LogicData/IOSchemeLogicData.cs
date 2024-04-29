using System;
using UnityEngine;

namespace Schemes.Data.LogicData
{
    [Serializable]
    public abstract class IOSchemeLogicData : SchemeLogicData, IOutputPortSchemeLogicData, IInputPortSchemesLogicData
    {
        [field:SerializeField] public byte NumberOfOutputs { get; private set; } // Note: may need public set accessor
        [field:SerializeField] public byte NumberOfInputs { get; private set; }  // Note: may need public set accessor

        // public byte[] Inputs;
        // [NonSerialized] public byte[] outputs;
        // public abstract byte[] GetOutputs();

    }
}