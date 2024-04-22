using System;
using Schemes.Data;

namespace Schemes
{
    [Serializable]
    public class Scheme
    {
        #region PRIVATE_VARIABLES

        private SchemeData _schemeData;
        
        #endregion

        #region GETTERS

        public SchemeData SchemeData => _schemeData;
        public SchemeKey SchemeKey => _schemeData.SchemeKey;

        #endregion

        public Scheme(SchemeData schemeData)
        {
            _schemeData = schemeData;
        }
    }
}
