using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Exceptions;
using Misc;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
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

        public List<Color> GetColors()
        {
            return paletteMaterials.Keys.Select(x => x.color).ToList();
        }

        #if UNITY_EDITOR

        public static SchemeMaterialPaletteSO GetSchemeMaterialPaletteConfig()
        {
            var assets = AssetDatabase.FindAssets("t:SchemeMaterialPaletteSO");
            if (assets.Length == 0)
            {
                throw new GameLogicException($"You have not created config of type {nameof(SchemePaletteConfigsContainer)} for scheme coloring.");
            }
            string filePath = AssetDatabase.GUIDToAssetPath(assets[0]);
            var schemeMaterialPaletteSo = AssetDatabase.LoadAssetAtPath<SchemeMaterialPaletteSO>(filePath);
            Debug.Log($"Using {schemeMaterialPaletteSo.name} as scheme material palette SO reference.");
            return schemeMaterialPaletteSo;
        }
        

        #endif

        public Color GetFirstColor()
        {
            return paletteMaterials.Keys.First();
        }
    }
}