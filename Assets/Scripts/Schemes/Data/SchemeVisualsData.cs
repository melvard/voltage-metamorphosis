using System;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public class SchemeVisualsData
    {
        private string _displayName;
        private Color _borderColor;
        
        private Vector2 _size;
        private Vector2[] _inputPositions;
        private Vector2[] _outputPositions;
        
    }
}