using System;
using System.Collections.Generic;
using System.Linq;
using Schemes.Data;
using Schemes.Data.LogicData;
using Schemes.Data.LogicData.Composition;
using Schemes.Data.LogicData.Relay;
using Schemes.Data.LogicData.UserIO;
using Schemes.Data.LogicData.Voltage;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Schemes.LogicUnit
{
    /// We are going to create instances of this struct to control electricity flows,
    /// each component in scheme should have this instance, even component schemes that are composition of
    /// other schemes should instantiate these objects on composed components.
    [Serializable]
    public class SchemeLogicUnit
    {
        public int index;
        public SchemeData UnderliningSchemeData;
        public List<SchemeLogicUnit> ComponentLogicUnits;
        public List<LogicUnitPort> Outputs;
        public List<LogicUnitPort> Inputs;
        public SchemeLogicData LogicData => UnderliningSchemeData.SchemeLogicData;

        public SchemeLogicUnit(SchemeData underliningSchemeData, int index)
        {
            this.index = index;
            UnderliningSchemeData = underliningSchemeData;
            Inputs = new();
            Outputs = new();

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
                
                
            if (underliningSchemeData.SchemeLogicData is CompositionLogicData compositionLogicData)
            {
                ComponentLogicUnits = compositionLogicData.GetComponentSchemes().Select(x=>x.InstantiateLogicUnit(-1)).ToList();
            }
            else
            {
                ComponentLogicUnits = new ();
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
                            // foreach (var outerRelation in outerRelations)
                            // {
                            //     if(outerRelation.receiverNode.
                            // }
                            // your code if is not defined
                            return;
                        }
                    }
                
                    if (Inputs[0].Value)
                    {
                        if (Inputs[1].Value)
                        {
                            Outputs[0] = new LogicUnitPort {Value = true, IsDefined = true};
                            Outputs[1] = new LogicUnitPort {Value = false, IsDefined = true};
                        }
                        else
                        {
                            Outputs[0] = new LogicUnitPort {Value = false, IsDefined = true};
                            Outputs[1] = new LogicUnitPort {Value = false, IsDefined = true};
                        }
                    }
                    else
                    {
                        if (Inputs[1].Value)
                        {
                            Outputs[0] = new LogicUnitPort {Value = false, IsDefined = true};
                            Outputs[1] = new LogicUnitPort {Value = true, IsDefined = true};
                        }
                        else
                        {
                            Outputs[0] = new LogicUnitPort {Value = false, IsDefined = true};
                            Outputs[1] = new LogicUnitPort {Value = false, IsDefined = true};
                        } 
                    }

                    break;
                }
                case PersistentVoltageLogicData:
                    Outputs[0] = new LogicUnitPort() {Value = true, IsDefined = true};
                    break;
                default:
                {
                    if (LogicData is CompositionLogicData)
                    {
                        // your logic here
                    }

                    break;
                }
            }
        }

        private void MarkNotConnectedInputsAsDefined(CompositionLogicData compositionLogicData)
        {
            foreach (var logicUnit in ComponentLogicUnits)
            {
                for (var i = 0; i < logicUnit.Inputs.Count; i++)
                {
                    logicUnit.Inputs[i] = new LogicUnitPort{ Value = false, IsDefined = true };
                }
            }
            foreach (var schemeRelation in compositionLogicData.SchemeRelations)
            {
                var receiver = ComponentLogicUnits[schemeRelation.receiverNode.ComponentIndexInComposition];
                receiver.Inputs[schemeRelation.receiverNode.ComponentPortIndex] =
                    new LogicUnitPort() { IsDefined = false };

                // ComponentLogicUnits[schemeRelation.receiverNode.ComponentIndexInComposition] = receiver;
            } 
        }

        private void ProcessRelations(CompositionLogicData compositionLogicData, List<SchemeRelation> relations, List<SchemeRelation> queuedRelations)
        {
            for (var i = 0; i < relations.Count; i++)
            {
                var logicDataSchemeRelation = relations[i];
                var senderNode = logicDataSchemeRelation.senderNode;
                var receiverNode = logicDataSchemeRelation.receiverNode;

                var senderLogicUnit = ComponentLogicUnits[senderNode.ComponentIndexInComposition];
                var receiverLogicUnit = ComponentLogicUnits[receiverNode.ComponentIndexInComposition];

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

                    continue;
                }

                // foreach (var ldsr in logicData.SchemeRelations)
                // {
                //     var ldsr_senderLogicUnit = ComponentLogicUnits[ldsr.senderNode.ComponentIndexInComposition];
                //     if (ldsr.receiverNode == receiverNode && ldsr.senderNode != senderNode)
                //     {
                //         ldsr_senderLogicUnit.Process();
                //     }
                //     receiverLogicUnit.Inputs[ldsr.receiverNode.ComponentPortIndex] = new LogicUnitPort()
                //     {
                //         Value = ldsr_senderLogicUnit.Outputs[senderNode.ComponentPortIndex].Value,
                //         IsDefined = true
                //     };
                // }
                // receiverLogicUnit.Process();
            }
        }

        // public bool GetOutputs(out bool wasDefined)
        // {
        //     wasDefined = true;
        //     if (LogicData is RelayLogicData relayLogicData)
        //     {
        //         foreach (var input in Inputs)
        //         {
        //             if (!input.IsDefined)
        //             {
        //                 wasDefined = false;
        //                 return false;
        //             }
        //         }
        //
        //         if (Inputs[0].Value)
        //         {
        //             if (Inputs[1].Value)
        //             {
        //                 // (portIndex == 0)
        //             }
        //         }
        //     }
        // }
       

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