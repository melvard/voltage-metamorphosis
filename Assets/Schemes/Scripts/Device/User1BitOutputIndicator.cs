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
        public void Init(UserOutputLogicData userOutputLogicData, int deviceIndex)
        {
            _userOutputLogicData = userOutputLogicData;
            _deviceIndex = deviceIndex;
            _value = false;
            _valueFromUserTextIndicator = Utilities.CreateWorldText(_value.ToString(), transform, new Vector3(0f, 0f, 1f));
            _valueFromUserTextIndicator.transform.localEulerAngles = new Vector3(90, 0f, 0f);
            _valueFromUserTextIndicator.fontSize = 10;
        }

        private void Update()
        {
            _value = GetLogicUnit().Inputs[0].Value;
            _valueFromUserTextIndicator.text = _value.ToString();
        }

        private SchemeLogicUnit GetLogicUnit()
        {
            return EditorDashboard.Instance.SchemeEditor_Debug.CurrentSchemeLogicUnit_Debug.ComponentLogicUnits.First(
                x => x.index == _deviceIndex);
        }
    }
}