using System;

namespace Schemes.Data.LogicData.Relay
{
    [Serializable]
    public class RelayLogicData : IOSchemeLogicData
    {
        // public override byte[] GetOutputs()
        // { 
        //     Inputs & (2 << 0);
        // }
        protected override SchemeLogicData GetCopy()
        {
            throw new NotImplementedException();
        }
    }
}