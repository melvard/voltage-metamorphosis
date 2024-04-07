using System;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public struct SchemeData
    {
        #region PRIVATE_VARIABLES

        [SerializeField] private SchemeKey schemeKey;
        [SerializeField] private string name;
        [SerializeField] private string description;
        
        [SerializeField] private SchemeVisualsData schemeVisualsData;
        [SerializeField] private SchemeLogicData schemeLogicData;

        public SchemeData(SchemeKey schemeKey)
        {
            this.schemeKey = schemeKey;
            this.name = "";
            description = "";
            schemeVisualsData = new();
            schemeLogicData = new();
        }

        #endregion

        #region GETTERS
        
        public string Name => name;
        public SchemeKey SchemeKey => schemeKey;

        #endregion
        
    }
}
