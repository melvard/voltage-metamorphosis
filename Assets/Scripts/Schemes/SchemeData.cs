using System;

namespace Schemes
{
    [Serializable]
    public struct SchemeData
    {
        private SchemeKey _schemeKey;
        private int _numberOfInputs;
        private int _numberOfOutputs;
        // relations here
        public SchemeKey SchemeKey => _schemeKey;
    }
}
