using System.Collections.Generic;
using Misc;

namespace Schemes
{
    public class SchemesContainer
    {
        private Dictionary<SchemeKey, Scheme> _schemeComponents;

        public SchemesContainer()
        {
            _schemeComponents = new();
        }
        public void AddSchemes(Scheme[] schemes)
        {
            foreach (var scheme in schemes)
            {
                if (!_schemeComponents.TryGetValue(scheme.SchemeKey, out var schemeInDict))
                {
                    _schemeComponents.Add(scheme.SchemeKey, scheme);
                }
                else
                {
                    
                }
            }
            //your code here to add scheme compnents
        }
        private void LoadSchemeComponents()
        {
            
        }
    }
}

