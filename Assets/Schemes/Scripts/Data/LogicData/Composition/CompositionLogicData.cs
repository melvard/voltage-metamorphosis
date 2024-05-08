using System;
using System.Collections.Generic;
using System.Linq;
using Exceptions;
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
        

        public static Scheme GetSchemeOfComponent(ComponentScheme componentScheme)
        {
            var schemesContainer = GameManager.Instance.GetContainerOfType<SchemesContainer>();
            return schemesContainer.GetSchemeByKey(componentScheme.SchemeKey);
        }

        public ComponentScheme ComponentByIndex(int componentIndexInComposition)
        {
            if (componentSchemes == null) throw new GameLogicException("Trying to get component scheme with component index, but list of components is null");
            foreach (var componentScheme in componentSchemes)
            {
                if (componentScheme.ComponentIndex == componentIndexInComposition) return componentScheme;
            }

            throw new GameLogicException($"Trying to get not included component of index {componentIndexInComposition} from {nameof(CompositionLogicData)}");
        }

        public void ThrowRelationsWithIndices(params int[] relationIndices)
        {
            for (var i = 0; i < schemeRelations.Count; i++)
            {
                var schemeRelation = schemeRelations[i];
                if (relationIndices.Any(index => index == schemeRelation.relationIndex))
                {
                    schemeRelations.RemoveAt(i);
                    i--;
                }
            }
        }

        protected override SchemeLogicData GetCopy()
        {
            var newCompositionLogicData = new CompositionLogicData()
            {
                componentSchemes = new(componentSchemes),
                schemeRelations = new(schemeRelations)
            };

            return newCompositionLogicData;
        }
    }
}