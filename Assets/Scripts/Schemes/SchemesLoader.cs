using System;
using Cysharp.Threading.Tasks;
using TreeEditor;

namespace Schemes
{
    public static class SchemesLoader
    {
        private static void LoadDefaultSchemes()
        {
            throw new NotImplementedException("Load default schemes is not implemented");
        }
        
        public static async UniTask<Scheme[]> LoadSchemes()
        {
            LoadDefaultSchemes();
            // deserialization here
            throw new NotImplementedException("Load schemes is not implemented");
        }

        public static async UniTask<Scheme> LoadSchemeByName()
        {
            // deserialization here
            throw new NotImplementedException("Load scheme by name is not implemented");
        }

        public static async UniTask<Scheme> LoadSchemeByGuid(string guidStr)
        {
            // deserialization here
            throw new NotImplementedException("Load scheme by guid is not implemented");
        }
        
    }
}