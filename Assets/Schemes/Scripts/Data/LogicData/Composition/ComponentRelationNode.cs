using System;
using UnityEngine;

namespace Schemes.Data.LogicData.Composition
{
    [Serializable]
    public class ComponentRelationNode
    {
        #region SERIALIZED_FIELDS

        [SerializeField] private int componentIndexInComposition = -1;
        [SerializeField] private byte componentPortIndex;

        #endregion

        
        #region GETTERS
        
        public int ComponentIndexInComposition => componentIndexInComposition;
        public int ComponentPortIndex => componentPortIndex;

        #endregion
        
        public ComponentRelationNode(int componentIndexInComposition, byte componentPortIndex)
        {
            this.componentIndexInComposition = componentIndexInComposition;
            this.componentPortIndex = componentPortIndex;
        }
    }
}