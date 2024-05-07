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
using Unity.VisualScripting;


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


        private List<SchemeRelation> _queuedSchemeRelations;

        public SchemeLogicUnit(SchemeData underliningSchemeData, int index, SchemeLogicUnit parentLogicUnit = null)
        {
            this.index = index;
            UnderliningSchemeData = underliningSchemeData;
            // Inputs = new();
            // Outputs = new();

            if (underliningSchemeData.SchemeLogicData is IInputPortSchemesLogicData inputPortSchemesLogicData)
            {
                GenerateInputUnitPorts(inputPortSchemesLogicData.NumberOfInputs);
            }
            if (underliningSchemeData.SchemeLogicData is IOutputPortSchemeLogicData outputPortSchemeLogicData)
            {
                GenerateOutputUnitPorts(outputPortSchemeLogicData.NumberOfOutputs);
            }

            if (underliningSchemeData.SchemeLogicData is not CompositionLogicData compositionLogicData) return;
            
            ComponentLogicUnits = compositionLogicData.ComponentSchemes
                .Where(componentScheme =>
                {
                    // effectively exclude User IO components if logic unit is of internal composition
                    var scheme = CompositionLogicData.GetSchemeOfComponent(componentScheme);
                    if (IsCurrentSchemeEditorLogicUnit) return true;
                    return scheme.SchemeData.SchemeLogicData is not (UserInputLogicData or UserOutputLogicData);
                })
                .Select(componentScheme =>
                {
                    var scheme = CompositionLogicData.GetSchemeOfComponent(componentScheme);
                    return new SchemeLogicUnit(scheme.SchemeData, componentScheme.ComponentIndex, this);
                }).ToList();

            _queuedSchemeRelations = new();
            
        }

        public void AlignInputsAndOutputsOnComponents(bool checkSelf = true)
        {
            if (UnderliningSchemeData.SchemeLogicData is CompositionLogicData)
            {
                if (checkSelf)
                {
                    AlignInputsAndOutputsWithParent();
                }
                foreach (var componentLogicUnit in ComponentLogicUnits)
                {
                    componentLogicUnit.AlignInputsAndOutputsWithParent();
                } 
            }
        }

        private void AlignInputsAndOutputsWithParent()
        {
            if (UnderliningSchemeData.SchemeLogicData is not CompositionLogicData compositionLogicData || IsCurrentSchemeEditorLogicUnit) return;

            foreach (var relation in compositionLogicData.SchemeRelations)
            {
                // Note: consider directly checking is relation receiver node is input for UserOutputDevice ot not
                var senderLogicUnitPos = ComponentLogicUnits.IndexOf(logicUnit =>
                    logicUnit.index == relation.senderNode.ComponentIndexInComposition);
                var receiverLogicUnitPos = ComponentLogicUnits.IndexOf(logicUnit =>
                    logicUnit.index == relation.receiverNode.ComponentIndexInComposition);
            
                if (receiverLogicUnitPos == -1)
                {
                    var numberOfOutput = UnderliningSchemeData.SchemeEditorData.outputEditorDatas.First(x =>
                        x.componentIndexInComposition == relation.receiverNode.ComponentIndexInComposition);
                    ComponentLogicUnits[senderLogicUnitPos].Outputs[relation.senderNode.ComponentPortIndex] = Outputs[numberOfOutput.numberOfIOForScheme];
                    
                    // if (goUnderChild)
                    // {
                    //     
                    // }
                }
            
                if (senderLogicUnitPos == -1)
                {
                    var numberOfInput = UnderliningSchemeData.SchemeEditorData.inputEditorDatas.First(x =>
                        x.componentIndexInComposition == relation.senderNode.ComponentIndexInComposition);
                        
                    ComponentLogicUnits[receiverLogicUnitPos].Inputs[relation.receiverNode.ComponentPortIndex] = Inputs[numberOfInput.numberOfIOForScheme];
                    
                    // if (goUnderChild)
                    // {
                    //     ComponentLogicUnits[receiverLogicUnitPos].AlignInputsAndOutputsWithParent(true);
                    // }
                }
            }

            AlignInputsAndOutputsOnComponents(false);
        }

        public void Process()
        {
            switch (LogicData)
            {
                case CompositionLogicData compositionLogicData:
                {
                    MarkNotConnectedInputsAsDefined(compositionLogicData);
                    _queuedSchemeRelations.Clear();
                    ProcessRelations(compositionLogicData.SchemeRelations, _queuedSchemeRelations);
                    while (_queuedSchemeRelations.Count != 0)
                    {
                        ProcessRelations(_queuedSchemeRelations, _queuedSchemeRelations);
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
                    Outputs[0].IsDefined = true;
                    break;
            }
        }


        private void GenerateOutputUnitPorts(int numberOfOutputPorts)
        {
            Outputs = new(numberOfOutputPorts);
            for (int i = 0; i < numberOfOutputPorts; i++)
            {
                Outputs.Add(new LogicUnitPort());
            }
        }

        private void GenerateInputUnitPorts(int numberOfInputPorts)
        {
            Inputs = new(numberOfInputPorts);
            for (int i = 0; i < numberOfInputPorts; i++)
            {
                Inputs.Add(new LogicUnitPort());
            }
        }
        private void MarkNotConnectedInputsAsDefined(CompositionLogicData compositionLogicData)
        {
            foreach (var logicUnit in ComponentLogicUnits)
            {
                if (logicUnit.LogicData is IInputPortSchemesLogicData)
                {
                    foreach (var logicUnitOutput in logicUnit.Inputs)
                    {
                        {
                            logicUnitOutput.IsDefined = true;
                        }
                    }
                }
            }

            foreach (var schemeRelation in compositionLogicData.SchemeRelations)
            {
                // Note: consider directly checking is relation receiver node is input for UserOutputDevice ot not
                var senderNodeIndex = ComponentLogicUnits.IndexOf(logicUnit=>logicUnit.index == schemeRelation.senderNode.ComponentIndexInComposition);
                var receiverNodeIndex = ComponentLogicUnits.IndexOf(logicUnit=>logicUnit.index == schemeRelation.receiverNode.ComponentIndexInComposition);
                
                // there is no receiver logic unit, thus this is a relation to UserOutputScheme
                if (senderNodeIndex != -1 && receiverNodeIndex != -1)
                {
                    ComponentLogicUnits[receiverNodeIndex].Inputs[schemeRelation.receiverNode.ComponentPortIndex].IsDefined = false;

                    // then assign Output of that port to Output of current logic unit
                    // ComponentLogicUnits[senderNodeIndex].Outputs[schemeRelation.senderNode.ComponentPortIndex] = Outputs[outputIndexOnScheme];
                    // ComponentLogicUnits[senderNodeIndex].Outputs[schemeRelation.senderNode.ComponentPortIndex].IsDefined == falsx    e;
                }
            }
        }

        private void ProcessRelations(List<SchemeRelation> relations,
            List<SchemeRelation> queuedRelations)
        {
            for (var i = 0; i < relations.Count; i++)
            {
                var relation = relations[i];
                var senderNode = relation.senderNode;
                var receiverNode = relation.receiverNode;

                // Note: consider directly checking is relation receiver node is input for UserOutputDevice ot not
                
                var senderLogicUnitPos = ComponentLogicUnits.IndexOf(logicUnit =>
                    logicUnit.index == relation.senderNode.ComponentIndexInComposition);
                var receiverLogicUnitPos = ComponentLogicUnits.IndexOf(logicUnit =>
                    logicUnit.index == relation.receiverNode.ComponentIndexInComposition);

                if (senderLogicUnitPos == -1 || receiverLogicUnitPos == -1) continue;
                
                var senderLogicUnit = ComponentLogicUnits.First(logicUnit=>logicUnit.index == senderNode.ComponentIndexInComposition);
                var receiverLogicUnit =  ComponentLogicUnits.First(logicUnit=>logicUnit.index == receiverNode.ComponentIndexInComposition);

                
                if (senderLogicUnit.Inputs == null || senderLogicUnit.Inputs.All(x => x.IsDefined))
                {
                    senderLogicUnit.Process();
                    
                    var receivingPort = receiverLogicUnit.Inputs[receiverNode.ComponentPortIndex];
                    receivingPort.Value = senderLogicUnit.Outputs[senderNode.ComponentPortIndex].Value;
                    receivingPort.IsDefined = true;
                }
                else
                {
                    // Note: does something missing? 

                    if (!queuedRelations.Contains(relation))
                    {
                        queuedRelations.Add(relation);
                    }

                    continue;
                }
                
                // continue;
                
                if (receiverLogicUnit.Inputs.All(x => x.IsDefined))
                {
                    if (queuedRelations.Contains(relation))
                    {
                        if (relations == queuedRelations) i--;
                        queuedRelations.Remove(relation);
                    }

                    receiverLogicUnit.Process();
                }
                else
                {
                    if(HasAnyNotDefinedInputPortMatchingSchemeInputPort(receiverLogicUnit)) continue;
                    
                    if (!queuedRelations.Contains(relation))
                    {
                        queuedRelations.Add(relation);
                    }
                }
            }
        }

        private bool HasAnyNotDefinedInputPortMatchingSchemeInputPort(SchemeLogicUnit componentLogicUnit)
        {
            var has = false;
            foreach (var logicUnitPort in componentLogicUnit.Inputs)
            {
                if (!logicUnitPort.IsDefined)
                {
                    if (Inputs!= null && Inputs.Contains(logicUnitPort))
                    {
                        has = true;
                    }
                    break;
                }
            }

            return has;
        }
        

        #region EXTERNAL_COMMUNICATION

        public void AddComponentLogicUnit(SchemeLogicUnit schemeLogicUnit)
        {
            ComponentLogicUnits.Add(schemeLogicUnit);
        }

        public void RemoveComponentLogicUnit(SchemeLogicUnit schemeLogicUnit)
        {
            ComponentLogicUnits.Remove(schemeLogicUnit);
        }

        #endregion

        public void RemoveComponentLogicUnitWithIndex(int deviceDeviceIndex)
        {
            for (var i = 0; i < ComponentLogicUnits.Count; i++)
            {
                var componentLogicUnit = ComponentLogicUnits[i];
                if (componentLogicUnit.index == deviceDeviceIndex)
                {
                    ComponentLogicUnits.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public class LogicUnitPort
    {
        public bool Value;
        public bool IsDefined;
    }
}