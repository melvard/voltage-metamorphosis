using System;
using System.Collections.Generic;
using Schemes.Data;
using UnityEngine;

namespace Schemes
{
    // [Serializable]
    // public class ScriptableScheme
    // {
    //     [SerializeField] private SchemeData schemeData;
    //
    //     public ScriptableScheme()
    //     {
    //         schemeData = new SchemeData(new SchemeKey(Guid.NewGuid()));
    //     }
    // }

    [CreateAssetMenu(fileName = "DefaultScriptableSchemes", menuName = "SchemeConfigs/DefaultScriptableSchemes",
        order = 1)]
    public class DefaultScriptableSchemesSO : ScriptableObject
    {
        // todo: come here only after visuals
        [SerializeField] private List<SchemeData> defaultSchemesDataList;

        public List<SchemeData> DefaultSchemesDataList => defaultSchemesDataList;
    }
}