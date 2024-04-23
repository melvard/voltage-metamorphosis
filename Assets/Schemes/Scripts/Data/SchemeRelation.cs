using System;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public struct SchemeRelation
    {
        [SerializeReference] public RelationNode leftNode;
        [SerializeReference] public RelationNode rightNode;
    }
}