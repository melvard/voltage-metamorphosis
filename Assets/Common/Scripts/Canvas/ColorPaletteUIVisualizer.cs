using System.Collections.Generic;
using Misc;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Canvas
{
    public  class ColorPaletteUIVisualizer : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [Tooltip("Container for generated color cells")]
        [SerializeField] private Transform container;
        
        [AssetsOnly][SerializeField] private ColorPaletteUICell colorPaletteUICellRef;
        [ChildGameObjectsOnly][SerializeField] private ColorPaletteUICell selectedPaletteUICell;

        #endregion

        #region EVENTS

        public event UnityAction<Color> OnColorChanged;
    
        #endregion


        #region PRIVATE_FIELDS

        private List<Color> _paletteColors;
        private List<ColorPaletteUICell> _colorPaletteUICells;

        #endregion

        #region GETTERS

        private List<Color> PaletteColors => _paletteColors;

        #endregion

        public void SetColorPalette(List<Color> colors)
        {
            container.DestroyChildren();
            _colorPaletteUICells = new();
            _colorPaletteUICells.Clear();
            _paletteColors = colors;
            
            var numberOfColorsInPalette = _paletteColors.Count;
              
            for (int i = 0; i < numberOfColorsInPalette; i++)
            {
                var colorPaletteCell = Instantiate(colorPaletteUICellRef, container);
                colorPaletteCell.Color = _paletteColors[i];
                colorPaletteCell.OnCellClicked += OnColorPaletteCellClickHandler;
                _colorPaletteUICells.Add(colorPaletteCell);
            }
        }

        
        private void OnColorPaletteCellClickHandler(Color color)
        {
            if(selectedPaletteUICell.Color == color) return;
            SetSelectedColor(color);
            OnColorChanged?.Invoke(color);
        }

        public void SetSelectedColor(Color color)
        {
            selectedPaletteUICell.Color = color; 
        }
    }
}