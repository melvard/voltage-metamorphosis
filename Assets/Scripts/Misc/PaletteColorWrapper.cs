using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    [Serializable]
    public struct PaletteColorWrapper
    {
        [ColorPalette("Schemes")]
        public Color color;

        private PaletteColorWrapper(Color color)
        {
            this.color = color;
        }
        
        
        public static bool operator == (PaletteColorWrapper paletteColorWrapper,  Color color)
        {
            return (paletteColorWrapper.color == color);
        }
        
        public static bool operator == (Color color,  PaletteColorWrapper paletteColorWrapper)
        {
            return (paletteColorWrapper.color == color);
        }
        
        public static bool operator !=(Color color, PaletteColorWrapper paletteColorWrapper)
        {
            return !(color == paletteColorWrapper);
        }
        
        public static bool operator !=(PaletteColorWrapper paletteColorWrapper, Color color)
        {
            return !(paletteColorWrapper.color == color);
        }
        
        public static explicit operator PaletteColorWrapper(Color color)
        {
            return new PaletteColorWrapper(color);
        }

        public static implicit operator Color(PaletteColorWrapper paletteColorWrapper)
        {
            return paletteColorWrapper.color;
        }
        
        public bool Equals(PaletteColorWrapper other)
        {
            return color.Equals(other.color);
        }

        public override bool Equals(object obj)
        {
            return obj is PaletteColorWrapper other && Equals(other);
        }

        public override int GetHashCode()
        {
            return color.GetHashCode();
        }
        
    }
}