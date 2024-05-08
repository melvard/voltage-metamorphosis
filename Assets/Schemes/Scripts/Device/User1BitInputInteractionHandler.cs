using System.Linq;
using Misc;
using Schemes.Dashboard;
using Schemes.Data.LogicData.UserIO;
using Schemes.LogicUnit;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Schemes.Device
{
    [RequireComponent(typeof(SchemeDevice))]
    public class User1BitInputInteractionHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private UserInputLogicData _userInputLogicData;
        
        private bool _value;
        private int _deviceIndex;
        private TextMeshPro _valueFromUserTextIndicator;
        // SchemeLogicUnit SchemeLogicUnit
        
        public void Init(UserInputLogicData userInputLogicData, int deviceIndex)
        {
            _deviceIndex = deviceIndex;
            _userInputLogicData = userInputLogicData;
            _value = false;
            _valueFromUserTextIndicator = Utilities.CreateWorldText(_value.ToString().ToUpper().ToUpper(), transform, new Vector3(0f, 0f, 0.7f));
            _valueFromUserTextIndicator.transform.localEulerAngles = new Vector3(90, 0f, 0f);
            _valueFromUserTextIndicator.fontSize = 3;
        }
        
        public void ToggleState()
        {
            _value = !_value;
            var a = GetLogicUnit();
            a.Outputs[0].IsDefined = true;
            a.Outputs[0].Value = _value;
            _valueFromUserTextIndicator.text = _value.ToString().ToUpper();
        

        }
        
        // todo: this is temporary solution to meet game requirements as soon as possible 
        private SchemeLogicUnit GetLogicUnit()
        {
            return EditorDashboard.Instance.SchemeEditor_Debug.CurrentSchemeLogicUnit_Debug.ComponentLogicUnits.First(
                x => x.index == _deviceIndex);
        }
        
        private bool _pendingForValueSet;
        private Vector3 _positionOnPointerDown;
        public void OnPointerDown(PointerEventData eventData)
        {
            _pendingForValueSet = true;
            _positionOnPointerDown = transform.position;
        }
        
        // տապոռ way of doing things...
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pendingForValueSet)
            {
                if (transform.position == _positionOnPointerDown)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        ToggleState();
                    }
                }
            }
            _pendingForValueSet = false;
            
        }
    }
}