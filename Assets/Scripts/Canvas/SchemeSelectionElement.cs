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
        [SerializeField] private Transform schemeDescriptionHintContainer;
        [SerializeField] private float thresholdTimeToShowHint;
        
        #endregion

        #region PRIVATE_FIELDS

        private Scheme _holdingScheme;
        private CancellationTokenSource _hintDescriptionTasKCancellationTokenSource;
        private CancellationTokenSource _removeSchemeTaskCancellationTokenSource;

        #endregion

        #region EVENTS

        public event UnityAction<SchemeInteractionEventArgs> OnSchemeEditBtnClick;
        public event UnityAction<SchemeInteractionEventArgs> OnSchemeSelectBtnClick;
        public event UnityAction<SchemeInteractionEventArgs> OnSchemeRemoveBtnClick;
        
        #endregion
        
        public void Init(Scheme scheme)
        {
            _hintDescriptionTasKCancellationTokenSource = new CancellationTokenSource();
            _removeSchemeTaskCancellationTokenSource = new CancellationTokenSource();
            
            _holdingScheme = scheme;
            rawImage.texture = scheme.UIRenderTexture;
            schemeName.text = scheme.SchemeData.Name;
            schemeDescription.text = scheme.SchemeData.Description;
            schemeDescriptionHintContainer.gameObject.SetActive(false);
            
            selectSchemeButton.onClick.AddListener(OnSchemeSelectButtonClickHandler);
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

        }

        // Method called when the mouse enters the UI element
        public async void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log("Mouse is over the UI element.");
            await ShowDescriptionHint(_hintDescriptionTasKCancellationTokenSource.Token);            
        }

        private async UniTask ShowDescriptionHint(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(thresholdTimeToShowHint));

            var mousePrevPos = Input.mousePosition;
            var mouseStartPos = Input.mousePosition;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!schemeDescriptionHintContainer.gameObject.activeSelf)
                {
                    schemeDescriptionHintContainer.gameObject.SetActive(true);
                }
                
                Vector3 mouseLastDelta = Input.mousePosition - mousePrevPos;
                Vector3 mouseTotalDelta = Input.mousePosition - mouseStartPos;
                mousePrevPos = Input.mousePosition;
                if (mouseLastDelta.magnitude > 20f || mouseTotalDelta.magnitude > 120f)
                {
                    schemeDescriptionHintContainer.gameObject.SetActive(false);
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
            
            _hintDescriptionTasKCancellationTokenSource.Cancel();
            _hintDescriptionTasKCancellationTokenSource.Dispose();
            _hintDescriptionTasKCancellationTokenSource = new CancellationTokenSource();
            
            // _cancellationTokenSource.Dispose();
            // _cancellationTokenSource = new CancellationTokenSource();
            schemeDescriptionHintContainer.gameObject.SetActive(false);
        }

        private void OnSchemeSelectButtonClickHandler()
        {
            OnSchemeSelectBtnClick?.Invoke(new SchemeInteractionEventArgs(_holdingScheme));
        }

        private void OnEditSchemeButtonClickHandler()
        {
            OnSchemeEditBtnClick?.Invoke(new SchemeInteractionEventArgs(_holdingScheme));
        }
        
        private async void OnSchemeRemoveButtonClickHandler()
        {
            if (await RemoveSchemePopup.Spawn(_removeSchemeTaskCancellationTokenSource.Token))
            {
                OnSchemeRemoveBtnClick?.Invoke(new SchemeInteractionEventArgs(_holdingScheme));
            }
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