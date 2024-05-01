using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using GameLogic;
using JetBrains.Annotations;
using Schemes;
using Schemes.Dashboard;
using Schemes.Data;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Canvas
{

    public class SchemeUIData
    {
        public string name;
        public string description;
        public Color color;
        public float xSize;
        public float ySize;
        public event UnityAction<SchemeUIData> OnDataChanged;

        public void OnNameSubmitHandler(string arg0)
        {
            name = arg0;
            OnDataChanged?.Invoke(this);
        }

        public void OnDescriptionSubmitHandler(string arg0)
        {
            description = arg0;
            OnDataChanged?.Invoke(this);
        }
        
        public void OnXSizeSubmitHandler(string arg0)
        {
            xSize = Single.Parse(arg0);
            OnDataChanged?.Invoke(this);
        }
        
        public void OnYSizeSubmitHandler(string arg0)
        {
            ySize = Single.Parse(arg0);
            OnDataChanged?.Invoke(this);
        }

        public void OnColorChangedHandler(Color arg0)
        {
            color = arg0;
            OnDataChanged?.Invoke(this);
        }
    }
    public class SchemeEditorUI : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [Title("General info")]
        [SerializeField] private TMP_InputField schemeNameInputField;
        [SerializeField] private TMP_InputField schemeDescriptionInputField;
        
        [Title("Visuals")]
        [SerializeField] private ColorPaletteUIVisualizer schemeColorPaletteUIVisualizer;

        [Header("Size")] 
        [SerializeField] private TMP_InputField xSizeInputField;
        [SerializeField] private TMP_InputField ySizeInputField;
        
        #endregion

        private Scheme _scheme;
        private SchemeUIData _schemeUIData;
        public void Init()
        {
            schemeColorPaletteUIVisualizer.Init();
            // xSizeInputField.on
        }

        private async void Start()
        {
            await UniTask.Yield();

            Init();
            SetScheme(EditorDashboard.Instance.SchemeEditor_Debug.CurrentScheme_Debug);
        }

        private void SubscribeToInputEvents()
        {
            schemeNameInputField.onSubmit.AddListener(_schemeUIData.OnNameSubmitHandler);
            schemeDescriptionInputField.onSubmit.AddListener(_schemeUIData.OnDescriptionSubmitHandler);
            xSizeInputField.onSubmit.AddListener(_schemeUIData.OnXSizeSubmitHandler);
            ySizeInputField.onSubmit.AddListener(_schemeUIData.OnYSizeSubmitHandler);
            schemeColorPaletteUIVisualizer.OnColorChanged += _schemeUIData.OnColorChangedHandler;
        }

        // private void UnsubscribeFromInputEvents()
        // {
        //     // schemeNameInputField.onSubmit.RemoveListener(_schemeUIData.OnNameSubmitHandler);
        //     // schemeDescriptionInputField.onSubmit.RemoveListener(_schemeUIData.OnDescriptionSubmitHandler);
        //     // xSizeInputField.onSubmit.RemoveListener(_schemeUIData.OnXSizeSubmitHandler);
        //     // ySizeInputField.onSubmit.RemoveListener(_schemeUIData.OnYSizeSubmitHandler);
        // }

        private void SetScheme(Scheme scheme)
        {
            // UnsubscribeFromInputEvents();
            _schemeUIData = scheme.SchemeData.GetSchemeUIData();
            ApplyUIData(_schemeUIData);
            SubscribeToInputEvents();
            
            var schemeColors = GameManager.Instance.GetContainerOfType<ConfigsContainer>()
                .SchemeMaterialPaletteSo.GetColors(); 
            schemeColorPaletteUIVisualizer.SetColorPalette(schemeColors);
        }

        private void ApplyUIData(SchemeUIData schemeUIData)
        {
            schemeNameInputField.text = schemeUIData.name;
            schemeDescriptionInputField.text = schemeUIData.description;

            xSizeInputField.text = schemeUIData.xSize.ToString(CultureInfo.InvariantCulture);
            ySizeInputField.text = schemeUIData.ySize.ToString(CultureInfo.InvariantCulture);

            schemeColorPaletteUIVisualizer.SetSelectedColor(schemeUIData.color);
        }
        
    }
}