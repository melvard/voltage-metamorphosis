using System;

namespace Schemes.Data.LogicData
{
    [Serializable]
    public abstract class SchemeLogicData
    {
        public static T NewLogicData<T>() where T : SchemeLogicData, new()
        {
            return new T();
        }
        
        
    }
}


