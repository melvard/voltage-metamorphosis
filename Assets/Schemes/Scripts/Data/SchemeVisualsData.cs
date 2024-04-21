using System;
using System.Net.NetworkInformation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public class SchemeVisualsData
    {
        [SerializeField] private string displayName;
        // [ColorPalette][SerializeField] private Color borderColor;
        
        [SerializeField] private Vector2 size;
        [SerializeField] private Vector2[] inputPositions;
        [SerializeField] private Vector2[] outputPositions;

        public string DisplayName => displayName;
        public Vector2 Size => size;

        public Vector2 GetInputPortPosition(int portIndex)
        {
            return inputPositions[portIndex];
        }

        public Vector2 GetOutputPortPosition(int portIndex)
        {
            return outputPositions[portIndex];
        }
    }
}