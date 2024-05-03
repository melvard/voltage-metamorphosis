using System;
using Canvas;
using Schemes.Data.LogicData;
using Schemes.Data.LogicData.Composition;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public class SchemeData
    {
        #region PRIVATE_VARIABLES

        [Title("@name", bold: true)]
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private SchemeKey schemeKey;
        [SerializeField] private bool isEditable;
        
        [SerializeField] private SchemeVisualsData schemeVisualsData;
        [ShowIf("isEditable")][SerializeField] private SchemeEditorData schemeEditorData;
        [SerializeReference] private SchemeLogicData schemeLogicData;

        public SchemeData()
        {
            
        }

        public SchemeData(SchemeKey schemeKey)
        {
            this.schemeKey = schemeKey;
            name = "";
            description = "";
            schemeVisualsData = new();
            schemeEditorData = new();
            schemeLogicData = new CompositionLogicData();
            isEditable = true;
        }
        
        
        #endregion

        #region GETTERS
        public string Name => name;
        public string Description => description;
        public SchemeKey SchemeKey => schemeKey;
        public SchemeVisualsData SchemeVisualsData => schemeVisualsData;
        public SchemeLogicData SchemeLogicData => schemeLogicData;
        
        public SchemeEditorData SchemeEditorData => schemeEditorData;

        #endregion

        public static SchemeData NewSchemeData<T>() where T: SchemeLogicData, new()
        {
            var schemeData = new SchemeData
            {
                name = "Scheme name",
                description = "Scheme description",
                schemeKey = SchemeKey.NewKey(),
                schemeEditorData = new SchemeEditorData(),
                schemeVisualsData = SchemeVisualsData.NewVisualsData(),
                schemeLogicData = SchemeLogicData.NewLogicData<T>()
            };
            return schemeData;
        }

        public SchemeUIData GetSchemeUIData()
        {
            var schemeUIData =  new SchemeUIData()
            {
                name = name,
                description = description,
                color = schemeVisualsData.DeviceBodyColor,
                xSize = schemeVisualsData.Size.x,
                ySize = schemeVisualsData.Size.y,
            };
            schemeUIData.OnDataChanged += OnDataChangedFromUIHandler;
            
            return schemeUIData;
        }

        private void OnDataChangedFromUIHandler(SchemeUIData schemeUIData)
        {
            name = schemeUIData.name;
            description = schemeUIData.description;
            schemeVisualsData.SetBodySize(schemeUIData.xSize, schemeUIData.ySize);
            schemeVisualsData.SetColor(schemeUIData.color);
        }
    }
}
