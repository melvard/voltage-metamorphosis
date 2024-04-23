using Exceptions;
using Schemes;
using UnityEditor;
using UnityEngine;

namespace GameLogic
{
    public class SchemePaletteConfigsContainer : MonoContainer
    {
        [SerializeField] private SchemeMaterialPaletteSO schemeMaterialPaletteSo;

        public SchemeMaterialPaletteSO SchemeMaterialPaletteSo => schemeMaterialPaletteSo;

        #region UNITY_EDITOR

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

        #endregion
    }
}