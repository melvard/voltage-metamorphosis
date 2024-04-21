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
        private string guidStr;
        

        public SchemeKey()
        {
            guidStr = Guid.NewGuid().ToString();
        }

        #endregion
        
        public override string ToString()
        {
            return guidStr;
        }
    }
}