using Misc;
using UnityEngine;
using UnityEngine.Events;

namespace GameLogic
{
    public enum ApplicationInputFocus
    {
        UI = 1<<5,
        SchemeEditor= 1<<6,
        Other
    }
    
    public static class InputsManager
    {
        #region CONSTS

        public const int UI_LAYER = 5;
        public const int SCHME_EDITOR_LAYER = 6;

        #endregion


        #region EVENTS

        public static event UnityAction<ApplicationInputFocus> OnApplicationInputFocusChanged;

        #endregion
        
        #region PRIVATE_FIELDS

        private static ApplicationInputFocus _applicationInputFocus;

        #endregion
        
        public static bool GetKeyDown(KeyCode keyCode, int callingFromLayer)
        {
            if (CheckIfLayerAlignsWithFocus(callingFromLayer))
            {
                return Input.GetKeyDown(keyCode);
            }

            return false;
        }
        
        public static bool GetKey(KeyCode keyCode, int callingFromLayer)
        {
            if (CheckIfLayerAlignsWithFocus(callingFromLayer))
            {
                return Input.GetKey(keyCode);
            }

            return false;
        }

        
        public static bool GetKeUp(KeyCode keyCode, int callingFromLayer)
        {
            if (CheckIfLayerAlignsWithFocus(callingFromLayer))
            {
                return Input.GetKeyUp(keyCode);
            }

            return false;
        }
        private static bool CheckIfLayerAlignsWithFocus(int callingFromLayer)
        {
            switch (callingFromLayer)
            {
                case UI_LAYER:
                    if (_applicationInputFocus == ApplicationInputFocus.UI) return true;
                    break;
                case  SCHME_EDITOR_LAYER:
                    if (_applicationInputFocus == ApplicationInputFocus.SchemeEditor) return true;
                    break;
                default:
                    if (_applicationInputFocus == ApplicationInputFocus.Other) return true;
                    break;
            }
            return false;
        }

        public static float GetAxis(string axisName, int callingFromLayer)
        {
            if (CheckIfLayerAlignsWithFocus(callingFromLayer))
            {
                return Input.GetAxis(axisName);
            }

            return 0f;
        }

        public static void SetApplicationInputFocus(ApplicationInputFocus applicationInputFocus)
        {
            if(_applicationInputFocus == applicationInputFocus) return;
            _applicationInputFocus = applicationInputFocus;
            OnApplicationInputFocusChanged?.Invoke(_applicationInputFocus);
        }
        
        public static void SetApplicationInputFocus(int layer)
        {
            ApplicationInputFocus applicationInputFocusToSet = layer switch
            {
                UI_LAYER => ApplicationInputFocus.UI,
                SCHME_EDITOR_LAYER => ApplicationInputFocus.SchemeEditor,
                _ => ApplicationInputFocus.Other
            };
            
            
            if(_applicationInputFocus == applicationInputFocusToSet) return;

            _applicationInputFocus = applicationInputFocusToSet;
            OnApplicationInputFocusChanged?.Invoke(_applicationInputFocus);
        }
        
    }
}