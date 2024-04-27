using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Schemes
{
    [Serializable]
    public struct SchemeKey
    {
        #region PRIVATE_VARIABLES

        [SerializeField] private MyGuid myGuid;
        
        #endregion

        public static implicit operator string(SchemeKey schemeKey)
        {
            return schemeKey.myGuid;
        }

        public override string ToString()
        {
            return myGuid;
        }

        public static SchemeKey NewKey()
        {
            SchemeKey schemeKey = new SchemeKey();
            schemeKey.myGuid = MyGuid.NewGuid();
            return schemeKey;
        }
    }

    [Serializable]
    public struct MyGuid
    {
        [DisableInPlayMode][DisableInEditorMode][ShowInInspector][SerializeField]
        private string guidStr;
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
        
        public static implicit operator string(MyGuid myGuid)
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
    }
}