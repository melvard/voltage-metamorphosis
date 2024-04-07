using System;
using NewtonSoft.Json;
using UnityEditor;

namespace Schemes
{
    [Serializable]
    public struct SchemeKey
    {
        private GUID _guid;
        private string _name;
    }
}