namespace Schemes
{
    public class Scheme
    {
        #region PRIVATE_VARIABLES

        private SchemeData _schemeData;
        
        #endregion

        #region GETTERS

        public SchemeData SchemeData => _schemeData;
        public SchemeKey SchemeKey => _schemeData.SchemeKey;

        #endregion
    }

    // public class ModularSchemeComponent : SchemeComponent
    // {
    //     
    // }
}
