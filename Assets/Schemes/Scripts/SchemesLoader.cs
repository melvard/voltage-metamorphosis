using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic;
using TreeEditor;

namespace Schemes
{
    public static class SchemesLoader
    {
        private static List<Scheme> GetDefaultSchemes()
        {
           var schemeDatas =  GameManager.Instance.GetContainerOfType<ConfigsContainer>().DefaultScriptableSchemesSo.DefaultSchemesDataList;
           List<Scheme> defaultSchemes = new();
           foreach (var schemeData in schemeDatas)
           {
               Scheme scheme = new Scheme(schemeData);
               
               defaultSchemes.Add(scheme);
           }

           return defaultSchemes;
        }
        
        public static async UniTask<List<Scheme>> LoadSchemes()
        {
            var defaultSchemes = GetDefaultSchemes();
            
            return defaultSchemes;
            // todo: deserialization from json file here 
        }

        // Note: will not be used probably
        public static async UniTask<Scheme> LoadSchemeByName()
        {
            // todo: deserialization from json file here 
            throw new NotImplementedException("Load scheme by name is not implemented");
        }

        public static async UniTask<Scheme> LoadSchemeByGuid(string guidStr)
        {
            // todo: deserialization from json file here 
            throw new NotImplementedException("Load scheme by guid is not implemented");
        }
        
    }
}