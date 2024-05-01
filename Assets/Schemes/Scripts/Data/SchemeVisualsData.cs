using System;
using System.Linq;
using GameLogic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public class SchemeVisualsData
    {
        [SerializeField] private string displayName;
        [ValidateInput("SetMaterialWithPaletteColor", "Color should be included in SchemeMaterialPalette config (1 config expected in assets)")]
        [ColorPalette("Schemes")][SerializeField] private Color deviceBodyColor;

        [InfoBox("Body material comes from SchemePaletteContainer")]
        [DisableIf("@true")][ShowInInspector][SerializeField] private Material deviceBodyMaterial;
        
        [SerializeField] private Vector2 size;
        [SerializeField] private Vector2[] inputPositions;
        [SerializeField] private Vector2[] outputPositions;


        #if UNITY_EDITOR

        private bool SetMaterialWithPaletteColor(Color color)
        {
            Debug.Log("SetMaterialWithPaletteColor validator invoked");
            var material = SchemeMaterialPaletteSO.GetSchemeMaterialPaletteConfig().GetSchemeMaterialWithPaletteColor(color);
            deviceBodyMaterial = material;
            if (material == null)
            {
                return false;
            }
            // AssetDatabase.FindAssets("");
            // deviceBodyMaterial 
            // deviceBodyMaterial = .GetContainer(); }
            return true;
        }
        
        #endif

        #region GEATTERS

        
        public string DisplayName => displayName;
        public Vector2 Size => size;

        public Material DeviceBodyMaterial => deviceBodyMaterial;

        public Vector2 GetInputPortPosition(int portIndex)
        {
            return inputPositions[portIndex];
        }

        public Vector2 GetOutputPortPosition(int portIndex)
        {
            return outputPositions[portIndex];
        }

        #endregion

        #region SETTERS

        public void SetDisplayName(string name)
        {
            displayName = name;
        }

        public void SetBodySize(Vector2 size)
        {
            this.size = size;
        }

        public void SetMaterial(Color color)
        {
            deviceBodyMaterial = GameManager.Instance
                .GetContainerOfType<ConfigsContainer>()
                .SchemeMaterialPaletteSo
                .GetSchemeMaterialWithPaletteColor(color);
        }
        
        public void SetInputPositions(Vector2[] inputPoses)
        {
            inputPositions = inputPoses;
        }

        public void SetOutputPositions(Vector2[] outputPoses)
        {
            outputPositions = outputPoses;
        }
        
        #endregion

        public static SchemeVisualsData NewVisualsData()
        {
            return new SchemeVisualsData();
        }

        public ColorPalette FetchColorPalette()
        {
             return Sirenix.OdinInspector.Editor.ColorPaletteManager.Instance.ColorPalettes
                .First(palette => palette == null);
        }
    }
}