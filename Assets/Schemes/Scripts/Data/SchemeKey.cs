using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Schemes
{
    [Serializable]
    public class SchemeKey
    {
        #region PRIVATE_VARIABLES

        [DisableInEditorMode][ShowInInspector][OdinSerialize]
        private string _guidStr;
        
        public SchemeKey()
        {
            _guidStr = Guid.NewGuid().ToString();
        }

        #endregion


        public static implicit operator string(SchemeKey schemeKey)
        {
            return schemeKey._guidStr;
        }
        
        public override string ToString()
        {
            return _guidStr;
        }
    }

}