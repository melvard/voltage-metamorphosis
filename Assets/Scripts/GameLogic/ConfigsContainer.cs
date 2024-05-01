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
        public DefaultScriptableSchemesSO DefaultScriptableSchemesSo;
        public SchemeMaterialPaletteSO SchemeMaterialPaletteSo;
    }
}