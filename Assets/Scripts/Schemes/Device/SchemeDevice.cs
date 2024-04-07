using Sirenix.OdinInspector;
using UnityEngine;

namespace Schemes.Device
{
    public class SchemeDevice : MonoBehaviour
    {
        [ShowInInspector][DisableInEditorMode]private Scheme _scheme;
        public Scheme scheme;
    }
}