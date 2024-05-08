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


        public static SchemeLogicData CopyFrom(SchemeLogicData schemeLogicData)
        {
            var copy = schemeLogicData.GetCopy();
            return copy;
        }

        protected abstract SchemeLogicData GetCopy();
    }
}


