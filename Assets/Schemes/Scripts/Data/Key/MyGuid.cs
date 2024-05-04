using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public struct MyGuid
    {
        [DisableInPlayMode][DisableInEditorMode][ShowInInspector][SerializeField]
        private string guidStr;

        public MyGuid(string guidString)
        {
            guidStr = guidString;
        }

        [ButtonGroup][Button("GenerateGUID")]
        private void GenerateGUID()
        {
            guidStr = Guid.NewGuid().ToString();
        }

#if UNITY_EDITOR
        [ButtonGroup][Button("Copy")]
        private void CopyGuidToClipboard()
        {
            EditorGUIUtility.systemCopyBuffer = guidStr;
        }
#endif
        
        public static explicit operator string(MyGuid myGuid)
        {
            return myGuid.guidStr;
        }
        
        public override string ToString()
        {
            return guidStr;
        }

        public static MyGuid NewGuid()
        {
            var myGuid = new MyGuid();
            myGuid.GenerateGUID();
            return myGuid;
        }

        #region OPERATOR_OVERRIDES

        public static bool operator ==(MyGuid a, MyGuid b)
        {
            return a.guidStr == b.guidStr;
        }

        public static bool operator !=(MyGuid a, MyGuid b)
        {
            return !(a == b);
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other is not MyGuid otherGuid) return false;
            return otherGuid == this;
        }

        public bool Equals(MyGuid other)
        {
            return guidStr == other.guidStr;
        }

        public override int GetHashCode()
        {
            return (guidStr != null ? guidStr.GetHashCode() : 0);
        }

        #endregion
    }
}