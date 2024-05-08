using System;
using System.Threading;
using Canvas.Popups;
using Cysharp.Threading.Tasks;
using Schemes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Canvas
{
    public class SchemeSelectionElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region SERIALIZED_FIELDS

        [SerializeField] private TextMeshProUGUI schemeName;
        [SerializeField] private RawImage rawImage;
        
        [Space][Title("Buttons")]
        [SerializeField] private Button selectSchemeButton;
        [SerializeField] private Transform schemeAlteringButtonsContainer;
        [SerializeField] private Button editButton;
        [SerializeField] private Button removeSchemeButton;
        
        [Space][Title("Description hint setting")]
        [SerializeField] private TextMeshProUGUI schemeDescription;
        [SerializeField] private Transform schemeDescriptionTooltipContainer;
        [SerializeField] private float thresholdTimeToShowHint;
        
        #endregion

        #region PRIVATE_FIELDS

        private Scheme _holdingScheme;
        private CancellationTokenSource _descriptionTooltipTasKCancellationTokenSource;
        private CancellationTokenSource _buttonTasksCancellationTokenSource;
        private bool _interactable = false;
        
        #endregion

        #region EVENTS

        public event UnityAction<SchemeInteractionEventArgs> OnSchemeEditBtnClick;
        public event UnityAction<SchemeInteractionEventArgs> OnSchemeSelectBtnClick;
        public event UnityAction<SchemeInteractionEventArgs> OnSchemeRemoveBtnClick;
        
        #endregion

        #region GETTERS

        public Scheme HoldingScheme => _holdingScheme;
        public bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                editButton.interactable = _interactable;
                removeSchemeButton.interactable = _interactable;
                selectSchemeButton.interactable = _interactable;
            }
        }

        #endregion
        
        public void Init(Scheme scheme)
        {
            Interactable = true;
            _descriptionTooltipTasKCancellationTokenSource = new CancellationTokenSource();
            _buttonTasksCancellationTokenSource = new CancellationTokenSource();

            RefreshData(scheme);
            
            if (scheme.SchemeData.IsEditable)
            {
                schemeAlteringButtonsContainer.gameObject.SetActive(true);
                editButton.onClick.AddListener(OnEditSchemeButtonClickHandler);
                removeSchemeButton.onClick.AddListener(OnSchemeRemoveButtonClickHandler);
            }
            else
            {
                schemeAlteringButtonsContainer.gameObject.SetActive(false);
            }
            
            schemeDescriptionTooltipContainer.gameObject.SetActive(false);
            selectSchemeButton.onClick.AddListener(OnSchemeSelectButtonClickHandler);
        }

        // Method called when the mouse enters the UI element
        public async void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log("Mouse is over the UI element.");
            await ShowDescriptionTooltip(_descriptionTooltipTasKCancellationTokenSource.Token);            
        }

        private async UniTask ShowDescriptionTooltip(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(thresholdTimeToShowHint));

            var mousePrevPos = Input.mousePosition;
            var mouseStartPos = Input.mousePosition;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!schemeDescriptionTooltipContainer.gameObject.activeSelf)
                {
                    schemeDescriptionTooltipContainer.gameObject.SetActive(true);
                }
                
                Vector3 mouseLastDelta = Input.mousePosition - mousePrevPos;
                Vector3 mouseTotalDelta = Input.mousePosition - mouseStartPos;
                mousePrevPos = Input.mousePosition;
                if (mouseLastDelta.magnitude > 20f || mouseTotalDelta.magnitude > 120f)
                {
                    schemeDescriptionTooltipContainer.gameObject.SetActive(false);
                    break;
                }
                await UniTask.Yield(cancellationToken);
            }
            // cancellationToken.ThrowIfCancellationRequested();
            // var localMousePosition = transform.InverseTransformPoint(Input.mousePosition);
            // localMousePosition.z = 0f;
            //
            // schemeDescriptionHintContainer.transform.localPosition = localMousePosition;
        }

        // Method called when the mouse exits the UI element
        public async void OnPointerExit(PointerEventData eventData)
        {
            // Debug.Log("Mouse has exited the UI element.");
            
            _descriptionTooltipTasKCancellationTokenSource.Cancel();
            _descriptionTooltipTasKCancellationTokenSource.Dispose();
            _descriptionTooltipTasKCancellationTokenSource = new CancellationTokenSource();
            
            // _cancellationTokenSource.Dispose();
            // _cancellationTokenSource = new CancellationTokenSource();
            schemeDescriptionTooltipContainer.gameObject.SetActive(false);
        }

        private void OnSchemeSelectButtonClickHandler()
        {
            OnSchemeSelectBtnClick?.Invoke(new SchemeInteractionEventArgs(_holdingScheme));
        }

        private async void OnEditSchemeButtonClickHandler()
        {
            if (await EditSchemePopup.Spawn(_buttonTasksCancellationTokenSource.Token))
            {
                OnSchemeEditBtnClick?.Invoke(new SchemeInteractionEventArgs(_holdingScheme));
            }
        }
        
        private async void OnSchemeRemoveButtonClickHandler()
        {
            if (await RemoveSchemePopup.Spawn(_buttonTasksCancellationTokenSource.Token))
            {
                OnSchemeRemoveBtnClick?.Invoke(new SchemeInteractionEventArgs(_holdingScheme));
            }
        }

        public void RefreshData(Scheme scheme)
        {
            _holdingScheme = scheme;
            rawImage.texture = _holdingScheme.SchemeData.SchemeVisualsData.UITexture2D;
            schemeName.text = _holdingScheme.SchemeData.Name;
            schemeDescription.text = _holdingScheme.SchemeData.Description;
        }
    }

    public class SchemeInteractionEventArgs : EventArgs
    {
        public Scheme scheme;
        public SchemeInteractionEventArgs(Scheme scheme)
        {
            this.scheme = scheme;
        }
    }
}