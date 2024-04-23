using System;
using System.Collections.Generic;
using GameLogic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Schemes.Data
{
    public enum SchemeLogicType
    {
        Relay,
        OneBitUserInput,
        OneBitOutput,
        Composition
    }

    [Serializable]
    public abstract class SchemeLogicData
    {
        
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


    public abstract class PersistentSchemeLogicData : SchemeLogicData, IOutputPortSchemeLogicData, IInputPortSchemesLogicData
    {
        [field:SerializeField] public byte NumberOfOutputs { get; private set; } // Note: may need set accessor
        [field:SerializeField] public byte NumberOfInputs { get; private set; }  // Note: may need set accessor
    }
    
    
    [Serializable]
    public class CompositionLogicData : PersistentSchemeLogicData
    {
        [SerializeField] private List<ComponentScheme> componentSchemes;
        [SerializeField] private List<SchemeRelation> schemeRelations;
    }
    
    [Serializable]
    public class RelayLogicData : PersistentSchemeLogicData
    {
        
    }

    

    [Serializable]
    public class PersistentVoltageLogicData : SchemeLogicData, IOutputPortSchemeLogicData
    {
        [field:SerializeField] public byte NumberOfOutputs { get; private set;  }
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


