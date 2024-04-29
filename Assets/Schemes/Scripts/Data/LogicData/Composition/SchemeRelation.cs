using System;
using UnityEngine;

namespace Schemes.Data.LogicData.Composition
{
    [Serializable]
    public struct SchemeRelation
    {
        [SerializeReference] public RelationNode rightNode;
        [SerializeReference] public RelationNode leftNode;
    }
}