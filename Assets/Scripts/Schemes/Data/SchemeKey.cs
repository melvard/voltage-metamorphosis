using System;
// using NewtonSoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Schemes
{
    [Serializable]
    public struct SchemeKey
    {
        #region PRIVATE_VARIABLES

        [DisableInEditorMode][ShowInInspector][OdinSerialize] private Guid _guid;

        public SchemeKey(Guid newGuid)
        {
            _guid = newGuid;
        }
        // [DisableInEditorMode][ShowInInspector][SerializeField] private string _name;

        #endregion

        #region GETTERS

        #endregion

        public override string ToString()
        {
            return _guid.ToString();
        }
    }
}