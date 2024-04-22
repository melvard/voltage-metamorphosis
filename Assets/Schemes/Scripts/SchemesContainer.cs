using System.Collections.Generic;
using GameLogic;
using Misc;
using Schemes.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Schemes
{
    public class SchemesContainer : IContainer
    {
        [ShowInInspector] private readonly Dictionary<string, Scheme> _schemeComponents;
        public SchemesContainer()
        {
            _schemeComponents = new();
        }
        public void AddSchemes(List<Scheme> schemes)
        {
            foreach (var scheme in schemes)
            {
                if (_schemeComponents.TryGetValue(scheme.SchemeKey, out var schemeInDict))
                {
                    Debug.Log($"Scheme with name {scheme.SchemeData.Name} already added. Refreshing scheme.");
                }
                _schemeComponents[scheme.SchemeKey] = scheme;
            }
        }

        public Scheme GetSchemeByKey(string schemeKeyGuid)
        {
            if (_schemeComponents.TryGetValue(schemeKeyGuid, out var scheme))
            {
                return scheme;
            }
            throw new KeyNotFoundException($"Scheme with key '{schemeKeyGuid}' is not found");
        }

        public void ClearContainer()
        {
            _schemeComponents.Clear();
        }
        
        // todo: container needs to be refreshed when scheme changes
       
    }
}

