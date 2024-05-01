using System.Collections.Generic;
using Misc;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace Canvas
{
    public  class ColorPaletteUIVisualizer : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [SerializeField] private ColorPaletteUICell colorPaletteUICellRef;
        [SerializeField] private Transform container;
        [SerializeField] private ColorPaletteUICell selectedPaletteUICell;

        #endregion

        #region EVENTS

        public event UnityAction<Color> OnColorChanged;
    
        #endregion


        #region PRIVATE_FIELDS

        private ColorPalette _colorPalette;
        private List<ColorPaletteUICell> _colorPaletteUICells;

        #endregion

        #region GETTERS

        private List<Color> PaletteColors => _colorPalette.Colors;

        #endregion

        public void Init()
        {
            _colorPaletteUICells = new();
        }

        public void SetColorPalette(List<Color> colors)
        {
            container.DestroyChildren();
            _colorPaletteUICells.Clear();
            
            _colorPalette = new ColorPalette()
            {
                Colors = colors
            };
            
            var numberOfColorsInPalette = _colorPalette.Colors.Count;
              
            for (int i = 0; i < numberOfColorsInPalette; i++)
            {
                var colorPaletteCell = Instantiate(colorPaletteUICellRef, container);
                colorPaletteCell.Color = _colorPalette.Colors[i];
                colorPaletteCell.OnCellClicked += OnColorPaletteCellClickHandler;
                _colorPaletteUICells.Add(colorPaletteCell);
            }
            selectedPaletteUICell.Color = _colorPaletteUICells[0].Color;
        }

        
        private void OnColorPaletteCellClickHandler(Color color)
        {
            selectedPaletteUICell.Color = color;
        }
    }
}