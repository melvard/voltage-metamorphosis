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
        [TextArea][SerializeField] private string description;
        [SerializeField] private SchemeKey schemeKey;
        [SerializeField] private bool isEditable;
        
        [SerializeField] private SchemeVisualsData schemeVisualsData;
        [ShowIf("isEditable")][SerializeField] private SchemeEditorData schemeEditorData;
        [SerializeReference] private SchemeLogicData schemeLogicData;

        

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

        public bool IsEditable => isEditable;

        #endregion

        public static SchemeData NewSchemeData<T>() where T: SchemeLogicData, new()
        {
            var schemeData = new SchemeData(SchemeKey.NewKey())
            {
                name = "Scheme name",
                description = "Scheme description",
                schemeEditorData = new SchemeEditorData(),
                schemeVisualsData = SchemeVisualsData.NewVisualsData(),
                schemeLogicData = SchemeLogicData.NewLogicData<T>()
            };

            if (schemeData.schemeLogicData is CompositionLogicData)
            {
                schemeData.isEditable = true;
            }
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
            
            return schemeUIData;
        }

        public void UpdateDataFromUI(SchemeUIData schemeUIData)
        {
            name = schemeUIData.name;
            schemeVisualsData.SetDisplayName(name);
            description = schemeUIData.description;
            schemeVisualsData.SetBodySize(schemeUIData.xSize, schemeUIData.ySize);
            schemeVisualsData.SetColor(schemeUIData.color);
        }
    }
}
