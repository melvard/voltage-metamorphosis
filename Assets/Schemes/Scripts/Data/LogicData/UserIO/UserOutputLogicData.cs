using System;
using UnityEngine;

namespace Schemes.Data.LogicData.UserIO
{
    [Serializable]
    public class UserOutputLogicData : SchemeLogicData, IInputPortSchemesLogicData
    {
        [field:SerializeField] public byte NumberOfInputs { get; private set; }
    }
}