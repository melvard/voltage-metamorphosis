using System;
using System.Globalization;
using System.Threading;
using Canvas.Popups;
using Cysharp.Threading.Tasks;
using GameLogic;
using Schemes;
using Schemes.Dashboard;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

        [Title("Size")] 
        [SerializeField] private TMP_InputField xSizeInputField;
        [SerializeField] private TMP_InputField ySizeInputField;

        [Title("Interaction button")] 
        [SerializeField] private Button saveSchemeButton;
        [SerializeField] private Button clearDashboardButton;
        [SerializeField] private Button newSchemeButton;

        [Title("Scheme selection section")] 
        [SerializeField] private SchemesSelectorUI schemesSelectorUI;
        
        #endregion


        #region PRIVATE_FIELDS

        private Scheme _scheme;
        private SchemeUIData _schemeUIData;
        private CancellationTokenSource _popupCancellationSource;

        #endregion

        #region EVENTS
        
        public event UnityAction<SchemeUIData> OnSaveSchemeCommandFromUI;
        public event UnityAction OnClearDashboardCommandFromUI;
        public event UnityAction OnNewSchemeCommandFromUI;
        
        #endregion
        // debugOnly: internal unity Start function is used to quickly test UI interactions 
        private async void Start()
        {
            await UniTask.Yield();
            await UniTask.Yield();
        }
        
        
        
        public void Init()
        {
            var schemesContainer = GameManager.Instance.GetContainerOfType<SchemesContainer>();
            
            schemesSelectorUI.Init(schemesContainer);
            schemesSelectorUI.OnSchemeSelectCommand += EditorDashboard.Instance.OnSchemeSelectCommandHandler;
            schemesSelectorUI.OnSchemeEditCommand += EditorDashboard.Instance.OnSchemeEditHandler;
            schemesSelectorUI.OnSchemeRemoveCommand += EditorDashboard.Instance.OnRemoveSchemeHandler;

            _popupCancellationSource = new CancellationTokenSource();
            
            SubscribeToButtonEvents();
            
            SetScheme(EditorDashboard.Instance.SchemeEditor_Debug.CurrentScheme_Debug);
            EditorDashboard.Instance.SchemeEditor_Debug.OnLoadedScheme += SetScheme;

        }

        
        private void SubscribeToButtonEvents()
        {
            saveSchemeButton.onClick.AddListener(OnSchemeSaveButtonClickHandler);
            newSchemeButton.onClick.AddListener(OnNewSchemeButtonClickHandler);
            clearDashboardButton.onClick.AddListener(OnClearDashboardButtonClickHandler);
        }

            
        private async void OnSchemeSaveButtonClickHandler()
        {
            if (await SavePopup.Spawn(_popupCancellationSource.Token))
            {
                OnSaveSchemeCommandFromUI?.Invoke(_schemeUIData);
            }
        }
        private async void OnNewSchemeButtonClickHandler()
        {
            if (await NewSchemePopup.Spawn(_popupCancellationSource.Token))
            {
                OnNewSchemeCommandFromUI?.Invoke();
            }
        }
        
        private async void OnClearDashboardButtonClickHandler()
        {
            if (await ClearDashboardPopup.Spawn(_popupCancellationSource.Token))
            {
                OnClearDashboardCommandFromUI?.Invoke();
            }
        }
        
        
        
        private void SubscribeToInputEvents()
        {
            schemeNameInputField.onSubmit.AddListener(_schemeUIData.OnNameSubmitHandler);
            schemeDescriptionInputField.onSubmit.AddListener(_schemeUIData.OnDescriptionSubmitHandler);
            xSizeInputField.onSubmit.AddListener(_schemeUIData.OnXSizeSubmitHandler);
            ySizeInputField.onSubmit.AddListener(_schemeUIData.OnYSizeSubmitHandler);
            
            schemeNameInputField.onEndEdit.AddListener(_schemeUIData.OnNameSubmitHandler);
            schemeDescriptionInputField.onEndEdit.AddListener(_schemeUIData.OnDescriptionSubmitHandler);
            xSizeInputField.onEndEdit.AddListener(_schemeUIData.OnXSizeSubmitHandler);
            ySizeInputField.onEndEdit.AddListener(_schemeUIData.OnYSizeSubmitHandler);
            
            
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
            schemesSelectorUI.PreventInteractionsWithScheme(scheme);
        }

        private void OnSchemeUIDataChangedHandler(SchemeUIData arg0)
        {
            
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