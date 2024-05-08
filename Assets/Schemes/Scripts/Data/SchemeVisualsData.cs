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
        #if UNITY_EDITOR
        [ValidateInput("SetMaterialWithPaletteColor", "Color should be included in SchemeMaterialPalette config (1 config expected in assets)")]
        #endif
        [ColorPalette("Schemes")][SerializeField] private Color deviceBodyColor;

        [InfoBox("Body material comes from SchemePaletteContainer")]
        [DisableIf("@true")][ShowInInspector][SerializeField] private Material deviceBodyMaterial;
        
        [SerializeField] private Vector2 size;
        [SerializeField] private Vector2[] inputPositions;
        [SerializeField] private Vector2[] outputPositions;
        
        [NonSerialized] private Texture2D _uITexture2D;

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

        #region GETTERS

        
        public string DisplayName => displayName;
        public Vector2 Size => size;

        public Material DeviceBodyMaterial
        {
            get
            {
                if (deviceBodyMaterial == null || deviceBodyMaterial.name == "HumanoidDefault")
                {
                    SetColor(deviceBodyColor);
                }
                return deviceBodyMaterial;
            }
        }

        public Color DeviceBodyColor => deviceBodyColor;

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
        
        public Texture2D UITexture2D
        {
            get => _uITexture2D;
            set => _uITexture2D = value;
        }

        public bool PendingForTextureCapture { get; set; }

        public void SetDisplayName(string name)
        {
            displayName = name;
        }

        public void SetBodySize(float xSize, float ySize)
        {
            SetBodySize(new Vector2(xSize, ySize));
        }
        public void SetBodySize(Vector2 size)
        {
            this.size = size;
        }
        
        public void SetColor(Color color)
        {
            deviceBodyColor = color;
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
            var schemeVisualsData =  new SchemeVisualsData()
            {
                size = new Vector2(1f, 1f),
            };
            schemeVisualsData.SetColor(GameManager.Instance
                .GetContainerOfType<ConfigsContainer>()
                .SchemeMaterialPaletteSo.GetFirstColor());

            return schemeVisualsData;
        }

        public ColorPalette FetchColorPalette()
        {
             return Sirenix.OdinInspector.Editor.ColorPaletteManager.Instance.ColorPalettes
                .First(palette => palette == null);
        }

        public void ArrangeInputPositionAutomatically(byte numberOfInputs)
        {
            inputPositions = new Vector2[numberOfInputs];
            var xPos = -size.x / 2f;
            var ySize=  size.y;
            var deltaPerPosition = ySize / (numberOfInputs+1);
            for (int i = 0; i < inputPositions.Length; i++)
            {
                inputPositions[i] = new Vector2(xPos, -ySize / 2f + deltaPerPosition * (i+1));
            }
        }

        public void ArrangeOutputPositionAutomatically(byte numberOfOutputs)
        {
            outputPositions = new Vector2[numberOfOutputs];
            var xPos = size.x / 2f;
            var ySize=  size.y;
            var deltaPerPosition = ySize / (numberOfOutputs+1);
            for (int i = 0; i < outputPositions.Length; i++)
            {
                outputPositions[i] = new Vector2(xPos, -ySize / 2f + deltaPerPosition * (i+1));
            }
        }

        public static SchemeVisualsData CopyFrom(SchemeVisualsData schemeVisualsData)
        {
            var copyVisualsData = new SchemeVisualsData()
            {
                displayName =  schemeVisualsData.displayName,
                deviceBodyColor = schemeVisualsData.deviceBodyColor,
                deviceBodyMaterial = schemeVisualsData.deviceBodyMaterial,
                size =  schemeVisualsData.size,
                inputPositions = new Vector2[schemeVisualsData.inputPositions.Length],
                outputPositions = new Vector2[schemeVisualsData.outputPositions.Length],
                _uITexture2D = schemeVisualsData._uITexture2D,
                PendingForTextureCapture = true
            };
            Array.Copy(schemeVisualsData.inputPositions, copyVisualsData.inputPositions, schemeVisualsData.inputPositions.Length);
            Array.Copy(schemeVisualsData.outputPositions, copyVisualsData.outputPositions, schemeVisualsData.outputPositions.Length);

            return copyVisualsData;
        }
    }
}