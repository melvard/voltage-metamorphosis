using System;
using System.Collections.Generic;
using GameLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public class SchemeLogicData
    {
        [SerializeField] private bool hasComponentSchemes;
        [ShowIf("hasComponentSchemes")]
        [SerializeField] private List<ComponentScheme> componentSchemes;
        
        [SerializeField] private byte numberOfInputs;
        [SerializeField] private byte numberOfOutputs;
        
        [SerializeField] private bool isRelayRelation = true;
        [ShowIf("@!isRelayRelation")]
        [SerializeField] private List<SchemeRelation> schemeRelations;

        public int NumberOfInputs => numberOfInputs;
        public int NumberOfOutputs => numberOfOutputs;
    }

    [Serializable]
    public struct SchemeRelation
    {
        [SerializeReference]public RelationNode leftNode;
        [SerializeReference]public RelationNode rightNode;
    }
    
    [Serializable]
    public abstract class RelationNode
    {
        
    }

    [Serializable]
    public class VoltageRelationNode : RelationNode
    {
        [SerializeField] private float voltage;
    }
    
    [Serializable]
    public class ComponentRelationNode : RelationNode
    {
        [SerializeField] private sbyte componentIndex = -1;
        [SerializeField] private byte componentPortIndex = 0;
    }

    // [Serializable]
    // public class InputRelationNode : ComponentRelationNode
    // {
    //     
    // }
    //
    // [Serializable]
    // public class OutputRelationNode : ComponentRelationNode
    // {
    //     
    // }


}


