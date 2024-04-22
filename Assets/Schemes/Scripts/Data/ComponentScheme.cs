using System;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public struct ComponentScheme
    {
        [SerializeField] private int componentIndex;
        [SerializeField] private SchemeKey schemeKey;
        
        // todo: Scheme get bt schemeKeyNum I guess
    }
}