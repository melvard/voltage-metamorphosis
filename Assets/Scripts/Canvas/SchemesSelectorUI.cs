using System.Collections.Generic;
using Misc;
using Schemes;
using Schemes.Dashboard;
using Schemes.Device;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Canvas
{
    public class SchemesSelectorUI : MonoBehaviour
    {
        [SerializeField] private Transform contentContainer;
        [AssetsOnly][SerializeField] private SchemeSelectionElement selectionElementContainerRef;
        

        public void UpdateSelector(List<Scheme> schemeInSelector)
        {
            contentContainer.DestroyChildren();
            
            foreach (var scheme in schemeInSelector)
            {
                var elementContainer = Instantiate(selectionElementContainerRef, contentContainer);
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