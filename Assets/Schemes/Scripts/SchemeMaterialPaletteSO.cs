using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Misc;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Schemes
{
    [CreateAssetMenu(fileName = "SchemeMaterialPalette", menuName = "SchemeConfigs/SchemeMaterialPalette",
        order = 2)]
    public class SchemeMaterialPaletteSO : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<PaletteColorWrapper, Material> paletteMaterials;

        public Material GetSchemeMaterialWithPaletteColor(Color color)
        {
            return paletteMaterials.TryGetValue((PaletteColorWrapper)color, out var material) ? material : null;
        }
    }
}