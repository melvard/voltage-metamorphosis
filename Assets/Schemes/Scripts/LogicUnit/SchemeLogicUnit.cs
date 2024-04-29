using System.Collections.Generic;
using System.Linq;
using Schemes.Data;
using Schemes.Data.LogicData;
using Schemes.Data.LogicData.Composition;

namespace Schemes.LogicUnit
{
    /// We are going to create instances of this struct to control electricity flows,
    /// each component in scheme should have this instance, even component schemes that are composition of
    /// other schemes should instantiate these objects on composed components.
    public struct SchemeLogicUnit
    {
        public SchemeData UnderliningSchemeData;
        public List<SchemeLogicUnit> ComponentLogicUnits;
        public List<LogicUnitPort> Outputs;
        public List<LogicUnitPort> Inputs;
        public SchemeLogicData LogicData => UnderliningSchemeData.SchemeLogicData;

        public SchemeLogicUnit(SchemeData underliningSchemeData)
        {
            UnderliningSchemeData = underliningSchemeData;
            Outputs = new();
            Inputs = new();
            if (underliningSchemeData.SchemeLogicData is CompositionLogicData compositionLogicData)
            {
                ComponentLogicUnits = compositionLogicData.GetComponentSchemes().Select(x=>x.InstantiateLogicUnit()).ToList();
            }
            else
            {
                ComponentLogicUnits = new ();
            }
        }


       

        #region EXTERNA_COMMUNICATION
        
        public void AddComponentLogicUnit(SchemeLogicUnit schemeLogicUnit)
        {
            ComponentLogicUnits.Add(schemeLogicUnit);
        }

        public void RemoveComponentLogicUnit(SchemeLogicUnit schemeLogicUnit)
        {
            ComponentLogicUnits.Remove(schemeLogicUnit);
        }
        
        #endregion
    }

    public struct LogicUnitPort
    {
        public bool Value;
        public bool IsDefined;
    }
}