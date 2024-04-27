using System;
using Schemes.Data;
using Sirenix.OdinInspector;

namespace Schemes
{
    [Serializable]
    public class Scheme
    {
        #region PRIVATE_VARIABLES

        [FoldoutGroup("Data")][DisableInPlayMode][DisableInEditorMode][ShowInInspector] private SchemeData _schemeData;
        
        #endregion

        #region GETTERS
        public SchemeData SchemeData => _schemeData;
        public SchemeKey SchemeKey => _schemeData.SchemeKey;

        #endregion

        public Scheme()
        {
            var schemeData = SchemeData.NewSchemeData<CompositionLogicData>();
            _schemeData = schemeData;
        }
        
        
        public Scheme(SchemeData schemeData)
        {
            _schemeData = schemeData;
        }
        
    }
}
