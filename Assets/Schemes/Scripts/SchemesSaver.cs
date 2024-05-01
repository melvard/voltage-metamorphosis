using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;


namespace Schemes
{
    public static class SchemesSaver
    {
        public static async UniTask<bool> SaveScheme(Scheme scheme)
        {
            var json = JsonConvert.SerializeObject(scheme);
            Debug.Log(json);
            
            throw new NotImplementedException("Save scheme is not implemented");
        }

        public static async UniTask<bool> SaveSchemes(List<Scheme> schemes)
        {
            throw new NotImplementedException("Save schemes is not implemented");
        }
    }
}