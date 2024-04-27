using System;
using System.Collections.Generic;
using Schemes.Data;
using UnityEngine;

namespace Schemes
{
    [CreateAssetMenu(fileName = "DefaultScriptableSchemes", menuName = "SchemeConfigs/DefaultScriptableSchemes",
        order = 1)]
    public class DefaultScriptableSchemesSO : ScriptableObject
    {
        // todo: come here only after visuals
        [SerializeField] private List<SchemeData> defaultSchemesDataList;

        public List<SchemeData> DefaultSchemesDataList => defaultSchemesDataList;
    }
}