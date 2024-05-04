using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using Schemes.LogicUnit;
using UnityEngine;

namespace Schemes.Data.LogicData.Composition
{
    [Serializable]
    public class CompositionLogicData : IOSchemeLogicData
    {
        [SerializeField] protected List<ComponentScheme> componentSchemes = new();
        [SerializeField] protected List<SchemeRelation> schemeRelations = new(); 

        public List<ComponentScheme> ComponentSchemes => componentSchemes;
        public List<SchemeRelation> SchemeRelations => schemeRelations;
        
        // public List<Scheme> GetComponentSchemes()
        // {
        //     return componentSchemes.Select(x => ).ToList();
        // }

        public static Scheme GetSchemeOfComponent(ComponentScheme componentScheme)
        {
            var schemesContainer = GameManager.Instance.GetContainerOfType<SchemesContainer>();
            return schemesContainer.GetSchemeByKey(componentScheme.SchemeKey);
        }
    }
}