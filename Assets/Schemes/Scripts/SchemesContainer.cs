using System.Collections.Generic;
using Misc;
using UnityEngine;

namespace Schemes
{
    public class SchemesContainer
    {
        private readonly Dictionary<SchemeKey, Scheme> _schemeComponents;
        public SchemesContainer()
        {
            _schemeComponents = new();
        }
        public void AddSchemes(Scheme[] schemes)
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

        public Scheme GetSchemeByKey(SchemeKey schemeKey)
        {
            if (_schemeComponents.TryGetValue(schemeKey, out var scheme))
            {
                return scheme;
            }
            throw new KeyNotFoundException($"Scheme with key '{schemeKey}' is not found");
        }

        public void ClearContainer()
        {
            _schemeComponents.Clear();
        }
        
        // todo: container needs to be refreshed when scheme changes
    }
}

