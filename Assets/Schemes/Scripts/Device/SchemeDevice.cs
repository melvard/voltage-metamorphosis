using System.Collections.Generic;
using Schemes.Data;
using Schemes.Device.Ports;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Schemes.Device
{
    
    [RequireComponent(typeof(ISchemeDeviceVisualizer))]
    public class SchemeDevice : MonoBehaviour
    {
        [SerializeField] private SchemeDeviceInputPort schemeDeviceInputPortRef;
        [SerializeField] private SchemeDeviceOutputPort schemeDeviceOutputPortRef;
        
        [Title("Info")]
        [ShowInInspector][DisableInEditorMode] private Scheme _underliningScheme;
        [ShowInInspector][DisableInEditorMode] private ISchemeDeviceVisualizer _schemeDeviceVisualizer;
        
        public void Init(Scheme scheme)
        {
            if (_schemeDeviceVisualizer == null)
            {
                _schemeDeviceVisualizer = GetComponent<ISchemeDeviceVisualizer>();
            }
            
            _underliningScheme = scheme;
            List<SchemeDeviceInputPort> schemeDeviceInputPorts = null;
            List<SchemeDeviceOutputPort> schemeDeviceOutputPorts = null;
            if (scheme.SchemeData.SchemeLogicData is IInputPortSchemesLogicData inputPortSchemesLogicData)
            {
                schemeDeviceInputPorts = GenerateInputPorts(inputPortSchemesLogicData.NumberOfInputs);
            }

            if (scheme.SchemeData.SchemeLogicData is IOutputPortSchemeLogicData outputPortSchemeLogicData)
            {
                schemeDeviceOutputPorts = GenerateOutputPorts(outputPortSchemeLogicData.NumberOfOutputs);
            }
       

            SchemeDeviceVisualsData schemeDeviceVisualsData = new()
            {   
                schemeVisualsData =  scheme.SchemeData.SchemeVisualsData,
                schemeDeviceInputPorts = schemeDeviceInputPorts,
                schemeDeviceOutputPorts = schemeDeviceOutputPorts
            };
            _schemeDeviceVisualizer.Visualise(schemeDeviceVisualsData);
            
            // _schemeLogicUnit.Logigalize(scheme.SchemeData.SchemeLogicData);
        }

        private List<SchemeDeviceInputPort> GenerateInputPorts(byte amountOfInputs)
        {
            List<SchemeDeviceInputPort> schemeDeviceInputPorts = new();
            for (int i = 0; i < amountOfInputs; i++)
            {
                var schemeDeviceInputPort = Instantiate(schemeDeviceInputPortRef, transform);
                schemeDeviceInputPorts.Add(schemeDeviceInputPort);
            }

            return schemeDeviceInputPorts;
        }

        private List<SchemeDeviceOutputPort> GenerateOutputPorts(byte amountOfInputs)
        {
            List<SchemeDeviceOutputPort> schemeDeviceOutputPorts = new();
            for (int i = 0; i < amountOfInputs; i++)
            {
                var schemeDeviceOutputPort = Instantiate(schemeDeviceOutputPortRef, transform);
                schemeDeviceOutputPorts.Add(schemeDeviceOutputPort);
            }

            return schemeDeviceOutputPorts;
        }
    }
}