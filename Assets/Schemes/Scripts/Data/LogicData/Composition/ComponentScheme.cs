using System;
using UnityEngine;

namespace Schemes.Data.LogicData.Composition
{
    [Serializable]
    public struct ComponentScheme
    {
        [SerializeField] private int componentIndex;
        [SerializeField] private SchemeKey schemeKey;

        public SchemeKey SchemeKey => schemeKey; 
        public int ComponentIndex => componentIndex; 
        public ComponentScheme(int componentIndex, SchemeKey schemeKey)
        {
            this.componentIndex = componentIndex;
            this.schemeKey = schemeKey;
        }
        // todo: Scheme get bt schemeKeyNum I guess
    }
}