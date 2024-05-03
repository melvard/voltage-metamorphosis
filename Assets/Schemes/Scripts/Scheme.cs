using System;
using Schemes.Data;
using Schemes.Data.LogicData.Composition;
using Schemes.LogicUnit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Schemes
{
    [Serializable]
    public class Scheme
    {
        #region PRIVATE_VARIABLES

        [FoldoutGroup("Data")][DisableInPlayMode][DisableInEditorMode][ShowInInspector][SerializeField] private SchemeData schemeData;
        [SerializeField] private RenderTexture uIRenderTexture;
        
        #endregion

        #region GETTERS
        public SchemeData SchemeData => schemeData;
        public SchemeKey SchemeKey => schemeData.SchemeKey;

        public RenderTexture UIRenderTexture
        {
            get => uIRenderTexture;
            set => uIRenderTexture = value;
        }

        #endregion

        public Scheme(SchemeData schemeData)
        {
            this.schemeData = schemeData;
        }

        public SchemeLogicUnit InstantiateLogicUnit(int index)
        {
            return new SchemeLogicUnit(schemeData, index);
        }

        #region OPERATOR_OVERRIDES

        public static bool operator ==(Scheme a, Scheme b)
        {
            return b != null && a != null && a.SchemeKey == b.SchemeKey;
        }

        public static bool operator !=(Scheme a, Scheme b)
        {
            return !(a == b);
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (other is not Scheme otherScheme) return false;
            return otherScheme == this;
        }

        protected bool Equals(Scheme other)
        {
            return Equals(schemeData, other.schemeData);
        }

        public override int GetHashCode()
        {
            return (schemeData != null ? schemeData.GetHashCode() : 0);
        }

        #endregion

        public static Scheme NewScheme()
        {
            var schemeData = SchemeData.NewSchemeData<CompositionLogicData>();
            return new Scheme(schemeData);
        }
    }
}
