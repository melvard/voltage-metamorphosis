using System;
using UnityEngine;

namespace Schemes.Data.LogicData.Composition
{
    [Serializable]
    public class ComponentRelationNode : RelationNode
    {
        [SerializeField] private int componentIndexInComposition = -1;
        [SerializeField] private byte componentPortIndex = 0;

        public ComponentRelationNode(int componentIndexInComposition, byte componentPortIndex)
        {
            this.componentIndexInComposition = componentIndexInComposition;
            this.componentPortIndex = componentPortIndex;
        }
    }
}