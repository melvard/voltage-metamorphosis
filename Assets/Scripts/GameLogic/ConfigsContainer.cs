using System.Collections.Generic;
using System.ComponentModel;
using Misc;
using Schemes;
using Schemes.Data;
using UnityEngine;

namespace GameLogic
{
    public class ConfigsContainer : MonoContainer
    {
        [SerializeField] private DefaultScriptableSchemes defaultScriptableSchemes;

        public List<SchemeData> GetDefaultSchemes()
        {
            
            return defaultScriptableSchemes.DefaultSchemesDataList;
            
        }
    }
}