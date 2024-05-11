using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Canvas
{
    public class ColorPaletteUICell : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image colorImage;

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                colorImage.color = _color;
            }
        }

        public event UnityAction<Color> OnCellClicked;
        private void Start()
        {
            button.onClick.AddListener(OnCellButtonClickHandler);
        }

        private void OnCellButtonClickHandler()
        {
            OnCellClicked?.Invoke(_color);
        }
    }
}