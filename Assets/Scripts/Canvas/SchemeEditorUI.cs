using System;
using Cysharp.Threading.Tasks;
using GameLogic;
using JetBrains.Annotations;
using Schemes;
using TMPro;
using UnityEngine;

namespace Canvas
{
    public class SchemeEditorUI : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [SerializeField] private TextMeshProUGUI schemeName;
        [SerializeField] private TextMeshProUGUI schemeDescription;
        [SerializeField] private ColorPaletteUIVisualizer schemeColorPaletteUIVisualizer;

        #endregion

        private Scheme _scheme;

        public void Init()
        {
            schemeColorPaletteUIVisualizer.Init();
        }

        private async void Start()
        {
            await UniTask.Yield();
            // debugonly
            Init();
            SetScheme(null);
        }

        private void SetScheme(Scheme scheme)
        {
            var schemeColors = GameManager.Instance.GetContainerOfType<ConfigsContainer>()
                .SchemeMaterialPaletteSo.GetColors(); 
            schemeColorPaletteUIVisualizer.SetColorPalette(schemeColors);
        }
        
    }
}