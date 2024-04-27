using System;
using System.Collections.Generic;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public abstract class SchemeLogicData
    {
        // returns 0/1
        public static T NewLogicData<T>() where T : SchemeLogicData, new()
        {
            return new T();
        }
    }
    
    
    [Serializable]
    public class UserInputLogicData : SchemeLogicData, IOutputPortSchemeLogicData
    {
        [field:SerializeField] public byte NumberOfOutputs { get; private set; }
    }
    
    [Serializable]
    public class OutputLogicDataData : SchemeLogicData, IInputPortSchemesLogicData
    {
        [field:SerializeField] public byte NumberOfInputs { get; private set; }
    }
    
    [Serializable]
    public abstract class IOSchemeLogicData : SchemeLogicData, IOutputPortSchemeLogicData, IInputPortSchemesLogicData
    {
        [field:SerializeField] public byte NumberOfOutputs { get; private set; } // Note: may need public set accessor
        [field:SerializeField] public byte NumberOfInputs { get; private set; }  // Note: may need public set accessor

        // public byte[] Inputs;
        // [NonSerialized] public byte[] outputs;
        // public abstract byte[] GetOutputs();

    }
    
    
    [Serializable]
    public class CompositionLogicData : IOSchemeLogicData
    {
        [SerializeField] protected List<ComponentScheme> componentSchemes = new();
        [SerializeField] protected List<SchemeRelation> schemeRelations = new(); 

        public List<ComponentScheme> ComponentSchemes => componentSchemes;
        public List<SchemeRelation> SchemeRelations => schemeRelations;
    }
    
    [Serializable]
    public class RelayLogicData : IOSchemeLogicData
    {
        // public override byte[] GetOutputs()
        // { 
        //     Inputs & (2 << 0);
        // }
    }

    

    [Serializable]
    public class PersistentVoltageLogicData : SchemeLogicData, IOutputPortSchemeLogicData
    {
        [field:SerializeField] public byte NumberOfOutputs { get; private set;  }
        // public override byte GetVoltage(int portIndex)
        // {
        //     return 1;
        // }
    }

    public interface IOutputPortSchemeLogicData
    {
        byte NumberOfOutputs { get; }
    }
    
    public interface IInputPortSchemesLogicData
    {
        byte NumberOfInputs { get; }
    }
}


