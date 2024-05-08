using System.Linq;
using Misc;
using Schemes.Dashboard;
using Schemes.Data.LogicData.UserIO;
using Schemes.LogicUnit;
using TMPro;
using UnityEngine;

namespace Schemes.Device
{
    [RequireComponent(typeof(SchemeDevice))]
    public class User1BitOutputIndicator : MonoBehaviour
    {
        private UserOutputLogicData _userOutputLogicData;
        private TextMeshPro _valueFromUserTextIndicator;

        private int _deviceIndex;
        private bool _value;
        private bool _prevValue;
        public void Init(UserOutputLogicData userOutputLogicData, int deviceIndex)
        {
            _userOutputLogicData = userOutputLogicData;
            _deviceIndex = deviceIndex;
            _prevValue = _value = false;
            _valueFromUserTextIndicator = Utilities.CreateWorldText(_value.ToString().ToUpper(), transform, new Vector3(0f, 0f, 0.7f));
            _valueFromUserTextIndicator.transform.localEulerAngles = new Vector3(90, 0f, 0f);
            _valueFromUserTextIndicator.fontSize = 3;
        }

        private void Update()
        {
            _value = GetLogicUnit().Inputs[0].Value;
            if (_prevValue != _value)
            {
                _valueFromUserTextIndicator.text = _value.ToString().ToUpper();
                _prevValue = _value;
            }
        }

        
        // todo: this is temporary solution to meet game requirements as soon as possible 
        private SchemeLogicUnit GetLogicUnit()
        {
            return EditorDashboard.Instance.SchemeEditor_Debug.CurrentSchemeLogicUnit_Debug.ComponentLogicUnits.First(
                x => x.index == _deviceIndex);
        }
    }
}