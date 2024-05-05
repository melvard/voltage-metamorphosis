using System.Collections.Generic;
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
        [SerializeField] private Transform contentContainer;
        [AssetsOnly][SerializeField] private SchemeSelectionElement selectionElementContainerRef;

        public event UnityAction<SchemeInteractionEventArgs> OnSchemeEditCommand;
        public event UnityAction<SchemeInteractionEventArgs> OnSchemeSelectCommand;
        public event UnityAction<SchemeInteractionEventArgs> OnSchemeRemoveCommand;
        public void UpdateSelector(List<Scheme> schemeInSelector)
        {
            contentContainer.DestroyChildren();
            
            foreach (var scheme in schemeInSelector)
            {
                var elementContainer = Instantiate(selectionElementContainerRef, contentContainer);
                elementContainer.OnSchemeEditBtnClick += (arg)=> OnSchemeEditCommand?.Invoke(arg);
                elementContainer.OnSchemeSelectBtnClick += (arg)=> OnSchemeSelectCommand?.Invoke(arg);
                elementContainer.OnSchemeRemoveBtnClick += (arg)=> OnSchemeRemoveCommand?.Invoke(arg);

                elementContainer.Init(scheme);
            } 
        }

        public void Init(SchemesContainer schemesContainer)
        {
            UpdateSelector(schemesContainer.GetSchemes());
            schemesContainer.OnSchemesChanged += UpdateSelector;
        }
    }
}