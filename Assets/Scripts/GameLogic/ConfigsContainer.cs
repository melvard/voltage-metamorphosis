using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Schemes;
using Schemes.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLogic
{
    public class ConfigsContainer : MonoContainer
    {
        [SerializeField] private DefaultScriptableSchemesSO defaultScriptableSchemesSo;

        public List<SchemeData> GetDefaultSchemes()
        {
            return defaultScriptableSchemesSo.DefaultSchemesDataList;
        }
    }
}