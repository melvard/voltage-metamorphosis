using System;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    
    public struct SchemeKey
    {
        #region CONSTS

        // debugOnly: these consts are for fast testing purposes

        public static readonly SchemeKey RELAY = new SchemeKey("d3336114-5910-42bd-b5e2-79518d3ff893");
        public static readonly SchemeKey ONE_BIT_USER_INPUT = new SchemeKey("2b738a45-ac8c-4508-8385-84491fe01b62");
        public static readonly SchemeKey ONE_BIT_OUTPUT = new SchemeKey("6c6dabff-4878-4697-8001-9ec948eed7cd");
        public static readonly SchemeKey CONSTANT_VOLTAGE = new SchemeKey("ee5cf222-b539-4e0c-a5b6-179a9561825d");

        #endregion
        
        #region PRIVATE_VARIABLES

        [SerializeField] private MyGuid myGuid;

        private SchemeKey(string guidString)
        {
            myGuid = new MyGuid(guidString);
        }

        #endregion

       
        public override string ToString()
        {
            return (string)myGuid;
        }

        public static SchemeKey NewKey()
        {
            SchemeKey schemeKey = new SchemeKey();
            schemeKey.myGuid = MyGuid.NewGuid();
            return schemeKey;
        }

        #region OPERATOR_OVERRIDES

        public static explicit operator string(SchemeKey schemeKey)
        {
            return (string)schemeKey.myGuid;
        }

        public static bool operator ==(SchemeKey a, SchemeKey b)
        {
            return a.myGuid == b.myGuid;
        }

        public static bool operator !=(SchemeKey a, SchemeKey b)
        {
            return !(a == b);
        }
        
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other is not SchemeKey otherSchemeKey) return false;
            return otherSchemeKey == this;
        }

        public bool Equals(SchemeKey other)
        {
            return myGuid.Equals(other.myGuid);
        }

        public override int GetHashCode()
        {
            return myGuid.GetHashCode();
        }

        #endregion

        public static SchemeKey CopyFrom(SchemeKey schemeKey)
        {
            var copyKey = new SchemeKey((string)schemeKey.myGuid);
            return copyKey;
        }
    }
}