using System.Collections.Generic;
using System.Linq;
using GameLogic;
using Misc;
using Schemes;
using Schemes.Data;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SchemesContainer : IContainer
{
    [ShowInInspector] private readonly Dictionary<SchemeKey, Scheme> _schemeComponents;
    public SchemesContainer()
    {
        _schemeComponents = new();
    }

    //Note: should be tested against performance issues 
    public event UnityAction<List<Scheme>> OnSchemesChanged; 

    public void AddSchemes(List<Scheme> schemes)
    {
        foreach (var scheme in schemes)
        {
            AddScheme(scheme);
        }
        OnSchemesChanged?.Invoke(schemes);
    }

    private void AddScheme(Scheme scheme)
    {
        if (_schemeComponents.TryGetValue(scheme.SchemeKey, out var schemeInDict))
        {
            Debug.Log($"Scheme with name {scheme.SchemeData.Name} already added. Refreshing scheme.");
        }
        _schemeComponents[scheme.SchemeKey] = scheme;
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

    public List<Scheme> GetSchemes()
    {
        return _schemeComponents.Select(x=>x.Value).ToList();
    }
}

namespace Schemes
{
}

