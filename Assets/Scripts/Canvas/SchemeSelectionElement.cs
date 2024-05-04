using System;
using System.Threading;
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
        [SerializeField] private Button editButton;
        [SerializeField] private Button selectSchemeButton;
        [Space][Title("Description hint setting")]
        [SerializeField] private TextMeshProUGUI schemeDescription;
        [SerializeField] private Transform schemeDescriptionHintContainer;
        [SerializeField] private float thresholdTimeToShowHint;
        
        #endregion

        #region PRIVATE_FIELDS

        private Scheme _holdingScheme;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region EVENTS

        public event UnityAction<SchemeInteractionEventArgs> OnSchemeEditBtnClick;
        public event UnityAction<SchemeInteractionEventArgs> OnSchemeSelectBtnClick;
        
        #endregion
        
        
        public void Init(Scheme scheme)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _holdingScheme = scheme;
            rawImage.texture = scheme.UIRenderTexture;
            schemeName.text = scheme.SchemeData.Name;
            schemeDescription.text = scheme.SchemeData.Description;
            schemeDescriptionHintContainer.gameObject.SetActive(false);
            
            editButton.onClick.AddListener(OnEditSchemeButtonClickHandler);
            selectSchemeButton.onClick.AddListener(OnSchemeSelectButtonClickHandler);
        }

        
        
        // Method called when the mouse enters the UI element
        public async void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log("Mouse is over the UI element.");
            await ShowDescriptionHint(_cancellationTokenSource.Token);            
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
            
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            
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