using System;
using System.Collections.Generic;
using System.Linq;
using Misc;
using Schemes.Data;
using Schemes.Data.LogicData;
using Schemes.Data.LogicData.Composition;
using Schemes.Data.LogicData.Relay;
using Schemes.Data.LogicData.UserIO;
using Schemes.Data.LogicData.Voltage;


namespace Schemes.LogicUnit
{
    /// We are going to create instances of this s̶t̶r̶u̶c̶t̶ class to control electricity flows,
    /// each component in scheme should have this instance, even component schemes that are composition of
    /// other schemes should instantiate these objects on composed components.
    /// The vast amount of responsibility for performance lies on this class.
    /// Consider being very careful when adding new feature and consider optimizing this class
    /// if you encounter with any performance issues.
    [Serializable]
    public class SchemeLogicUnit
    {
        public int index;
        public SchemeData UnderliningSchemeData;
        public List<SchemeLogicUnit> ComponentLogicUnits;
        public List<LogicUnitPort> Outputs;
        public List<LogicUnitPort> Inputs;

        #region GETTERS

        public SchemeLogicData LogicData => UnderliningSchemeData.SchemeLogicData;
        public bool IsCurrentSchemeEditorLogicUnit => index == -1;

        #endregion


        public SchemeLogicUnit(SchemeData underliningSchemeData, int index)
        {
            this.index = index;
            UnderliningSchemeData = underliningSchemeData;
            Inputs = new();
            Outputs = new();

            if (!IsCurrentSchemeEditorLogicUnit)
            {
                if (underliningSchemeData.SchemeLogicData is IInputPortSchemesLogicData inputPortSchemesLogicData)
                {
                    Inputs = new(inputPortSchemesLogicData.NumberOfInputs);
                    for (int i = 0; i < inputPortSchemesLogicData.NumberOfInputs; i++)
                    {
                        Inputs.Add(new LogicUnitPort());
                    }
                }

                if (underliningSchemeData.SchemeLogicData is IOutputPortSchemeLogicData outputPortSchemeLogicData)
                {
                    Outputs = new(outputPortSchemeLogicData.NumberOfOutputs);
                    for (int i = 0; i < outputPortSchemeLogicData.NumberOfOutputs; i++)
                    {
                        Outputs.Add(new LogicUnitPort());
                    }
                }
            }

            if (underliningSchemeData.SchemeLogicData is CompositionLogicData compositionLogicData)
            {
                ComponentLogicUnits = compositionLogicData.ComponentSchemes
                    .Where(componentScheme =>
                    {
                        var scheme = CompositionLogicData.GetSchemeOfComponent(componentScheme);
                        if (IsCurrentSchemeEditorLogicUnit) return true;
                        return scheme.SchemeData.SchemeLogicData is not (UserInputLogicData or UserOutputLogicData);
                    })
                    .Select(componentScheme =>
                    {
                        var scheme = CompositionLogicData.GetSchemeOfComponent(componentScheme);
                        return new SchemeLogicUnit(scheme.SchemeData, componentScheme.ComponentIndex);
                    }).ToList();
            }
            else
            {
                ComponentLogicUnits = new();
            }
        }

        public void Process()
        {
            switch (LogicData)
            {
                case CompositionLogicData logicData:
                {
                    MarkNotConnectedInputsAsDefined(logicData);
                    List<SchemeRelation> queuedRelations = new List<SchemeRelation>();
                    ProcessRelations(logicData, logicData.SchemeRelations, queuedRelations);
                    while (queuedRelations.Count != 0)
                    {
                        ProcessRelations(logicData, queuedRelations, queuedRelations);
                    }

                    break;
                }
                case RelayLogicData:
                {
                    foreach (var input in Inputs)
                    {
                        if (!input.IsDefined)
                        {
                            return;
                        }
                    }
                    Outputs[0].Value = Inputs[0].Value && Inputs[1].Value;
                    Outputs[0].IsDefined = true;
                    Outputs[1].Value = !Inputs[0].Value && Inputs[1].Value;
                    Outputs[1].IsDefined = true;
                    break;
                }
                case PersistentVoltageLogicData:
                    Outputs[0].Value = true;
                    Outputs[0].IsDefined = true;
                    break;
                case UserInputLogicData:
                    // Outputs[0].IsDefined = true;
                    break;
            }
        }

        private void MarkNotConnectedInputsAsDefined(CompositionLogicData compositionLogicData)
        {
            foreach (var logicUnit in ComponentLogicUnits)
            {
                foreach (var logicUnitOutput in logicUnit.Inputs)
                {
                    if (logicUnit.LogicData is UserInputLogicData)
                    {
                        
                    }
                    logicUnitOutput.Value = false;
                    logicUnitOutput.IsDefined = true;
                }

            }

            foreach (var schemeRelation in compositionLogicData.SchemeRelations)
            {
                // Note: consider directly checking is relation receiver node is input for UserOutputDevice ot not
                var senderNodeIndex = ComponentLogicUnits.IndexOf(logicUnit=>logicUnit.index == schemeRelation.senderNode.ComponentIndexInComposition);
                var receiverNodeIndex = ComponentLogicUnits.IndexOf(logicUnit=>logicUnit.index == schemeRelation.receiverNode.ComponentIndexInComposition);
                
                // there is no receiver logic unit, thus this is a relation to UserOutputScheme
                if (receiverNodeIndex == -1)
                {
                    // then assign Output of that port to Output of current logic unit
                    // ComponentLogicUnits[senderNodeIndex].Outputs[schemeRelation.senderNode.ComponentPortIndex] = Outputs[outputIndexOnScheme];
                    // ComponentLogicUnits[senderNodeIndex].Outputs[schemeRelation.senderNode.ComponentPortIndex].IsDefined == falsx    e;
                }
                else
                {
                    ComponentLogicUnits[receiverNodeIndex].Inputs[schemeRelation.receiverNode.ComponentPortIndex].IsDefined = false;
                }
                
                // there is no sender logic unit, thus this is a relation from UserInputScheme
                if (senderNodeIndex == -1)
                {
                    // then assign Input of that port to Input of current logic unit
                    // ComponentLogicUnits[receiverNodeIndex].Inputs[schemeRelation.receiverNode.ComponentPortIndex] = Inputs[inputIndexOnScheme];
                }
            }
        }

        private void ProcessRelations(CompositionLogicData compositionLogicData, List<SchemeRelation> relations,
            List<SchemeRelation> queuedRelations)
        {
            for (var i = 0; i < relations.Count; i++)
            {
                var logicDataSchemeRelation = relations[i];
                var senderNode = logicDataSchemeRelation.senderNode;
                var receiverNode = logicDataSchemeRelation.receiverNode;
                
                var senderLogicUnit = ComponentLogicUnits.First(logicUnit=>logicUnit.index == senderNode.ComponentIndexInComposition);
                var receiverLogicUnit =  ComponentLogicUnits.First(logicUnit=>logicUnit.index == receiverNode.ComponentIndexInComposition);

                if (senderLogicUnit.Inputs.Count == 0 || senderLogicUnit.Inputs.All(x => x.IsDefined))
                {
                    if (senderLogicUnit.Outputs.Any(x => !x.IsDefined))
                    {
                        senderLogicUnit.Process();
                    }

                    receiverLogicUnit.Inputs[receiverNode.ComponentPortIndex] = new LogicUnitPort()
                    {
                        Value = senderLogicUnit.Outputs[senderNode.ComponentPortIndex].Value,
                        IsDefined = true
                    };
                }
                else
                {
                    if (!queuedRelations.Contains(logicDataSchemeRelation))
                    {
                        queuedRelations.Add(logicDataSchemeRelation);
                    }

                    continue;
                }


                if (receiverLogicUnit.Inputs.All(x => x.IsDefined))
                {
                    if (queuedRelations.Contains(logicDataSchemeRelation))
                    {
                        if (relations == queuedRelations) i--;
                        queuedRelations.Remove(logicDataSchemeRelation);
                    }

                    receiverLogicUnit.Process();
                }
                else
                {
                    if (!queuedRelations.Contains(logicDataSchemeRelation))
                    {
                        queuedRelations.Add(logicDataSchemeRelation);
                    }
                }
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

    public class LogicUnitPort
    {
        public bool Value;
        public bool IsDefined;
    }
}