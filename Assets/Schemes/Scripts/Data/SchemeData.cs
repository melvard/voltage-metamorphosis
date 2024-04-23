using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public struct SchemeData
    {
        #region PRIVATE_VARIABLES

        [Title("@name", bold: true)]
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private SchemeKey schemeKey;
        
        [SerializeField] private SchemeVisualsData schemeVisualsData;
        [SerializeReference] private SchemeLogicData schemeLogicData;

        public SchemeData(SchemeKey schemeKey)
        {
            this.schemeKey = schemeKey;
            name = "";
            description = "";
            schemeVisualsData = new();
            schemeLogicData = new CompositionLogicData();
        }
        
        #endregion

        #region GETTERS
        public string Name => name;
        public SchemeKey SchemeKey => schemeKey;
        public SchemeVisualsData SchemeVisualsData => schemeVisualsData;
        public SchemeLogicData SchemeLogicData => schemeLogicData;

        #endregion

    }
}
