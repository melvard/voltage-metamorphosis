using System;
using UnityEngine;

namespace Schemes.Data.LogicData.UserIO
{
    [Serializable]
    public class UserInputLogicData : SchemeLogicData, IOutputPortSchemeLogicData
    {
        [field:SerializeField] public byte NumberOfOutputs { get; private set; }

        public byte Value { get; set; }

        public byte GetOutput()
        {
            return Value;
        }
    }
}