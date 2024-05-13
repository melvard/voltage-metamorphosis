using UnityEngine;

namespace Common.Misc
{
    public class MonoSingleton<T> : MonoBehaviour  where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>(true);
                }
                return _instance ;
            }
        }

        private static T _instance;
    }
}