using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Schemes.Data.LogicData.Composition
{
    [Serializable]
    public struct SchemeRelation
    {
        [SerializeField] public int relationIndex;
        [SerializeReference] public ComponentRelationNode senderNode;
        [SerializeReference] public ComponentRelationNode receiverNode;
    }
}