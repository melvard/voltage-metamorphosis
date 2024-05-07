using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Misc;
using Schemes;
using Schemes.Dashboard;
using Schemes.Device;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Canvas
{
    public class SchemesSelectorUI : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [SerializeField] private Transform contentContainer;
        [AssetsOnly][SerializeField] private SchemeSelectionElement selectionElementContainerRef;

        #endregion

        #region EVENTS

        public event UnityAction<SchemeInteractionEventArgs> OnSchemeEditCommand;
        public event UnityAction<SchemeInteractionEventArgs> OnSchemeSelectCommand;
        public event UnityAction<SchemeInteractionEventArgs> OnSchemeRemoveCommand;

        #endregion

        #region PRIVATE_FIELDS

        private List<SchemeSelectionElement> _schemeSelectionElements;

        #endregion
        public void Init(SchemesContainer schemesContainer)
        {
            _schemeSelectionElements = new();
            SchemesSaverLoader.OnSchemeAdded += OnSchemeAddedHandler;
            SchemesSaverLoader.OnSchemeRemoved += OnSchemeRemovedHandler;
            SchemesSaverLoader.OnSchemeEdited += OnSchemeEditedHandler;
            
            UpdateSelector(schemesContainer.GetSchemes());
        }
        
        public void UpdateSelector(List<Scheme> schemeInSelector)
        {
            contentContainer.DestroyChildren();
            
            foreach (var scheme in schemeInSelector)
            {
                AddSchemeInSelector(scheme);
            } 
        }

        private void AddSchemeInSelector(Scheme scheme)
        {
            var schemeSelectionElement = Instantiate(selectionElementContainerRef, contentContainer);
            
            schemeSelectionElement.OnSchemeEditBtnClick += (arg)=> OnSchemeEditCommand?.Invoke(arg);
            schemeSelectionElement.OnSchemeSelectBtnClick += (arg)=> OnSchemeSelectCommand?.Invoke(arg);
            schemeSelectionElement.OnSchemeRemoveBtnClick += (arg)=> OnSchemeRemoveCommand?.Invoke(arg);

            _schemeSelectionElements.Add(schemeSelectionElement);
            schemeSelectionElement.Init(scheme);
        }

        private void OnSchemeAddedHandler(SchemeInteractionEventArgs arg0)
        {
            AddSchemeInSelector(arg0.scheme);
        }

        private void OnSchemeRemovedHandler(SchemeInteractionEventArgs arg0)
        {
            var schemeSelectionElementIndex = _schemeSelectionElements.IndexOf(x => x.HoldingScheme == arg0.scheme);
            if (schemeSelectionElementIndex != -1)
            {
                Destroy(_schemeSelectionElements[schemeSelectionElementIndex].gameObject);
                _schemeSelectionElements.RemoveAt(schemeSelectionElementIndex);
            }
        }
        
        private async void OnSchemeEditedHandler(SchemeInteractionEventArgs arg0)
        {
            await UniTask.WaitUntil(() => arg0.scheme.SchemeData.SchemeVisualsData.PendingForTextureCapture == false);
            var schemeSelectionElementIndex = _schemeSelectionElements.IndexOf(x => x.HoldingScheme == arg0.scheme);
            if (schemeSelectionElementIndex != -1)
            {
                _schemeSelectionElements[schemeSelectionElementIndex].RefreshData();
            }
        }


        public void PreventInteractionsWithScheme(Scheme scheme)
        {
            foreach (var schemeSelectionElement in _schemeSelectionElements)
            {
                schemeSelectionElement.Interactable = true;
            } 
            var schemeSelectionElementIndex = _schemeSelectionElements.IndexOf(x => x.HoldingScheme == scheme);
            if (schemeSelectionElementIndex != -1)
            {
                _schemeSelectionElements[schemeSelectionElementIndex].Interactable = false;
            }
            
        }
    }
}