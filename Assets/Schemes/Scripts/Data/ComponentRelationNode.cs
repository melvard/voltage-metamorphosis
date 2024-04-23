using System;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public class ComponentRelationNode : RelationNode
    {
        [SerializeField] private sbyte componentIndex = -1;
        [SerializeField] private byte componentPortIndex = 0;
    }
}