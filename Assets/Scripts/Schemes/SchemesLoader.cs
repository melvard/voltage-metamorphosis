using System;
using Cysharp.Threading.Tasks;
using TreeEditor;

namespace Schemes
{
    public static class SchemesLoader
    {
        private static Scheme[] LoadDefaultSchemes()
        {
            throw new NotImplementedException("Load default schemes is not implemented");
        }
        
        public static async UniTask<Scheme[]> LoadSchemes()
        {
            throw new NotImplementedException("Load schemes is not implemented");
        }

        public static async UniTask<Scheme> LoadSchemeByName()
        {
            throw new NotImplementedException("Load scheme by name is not implemented");
        }

        public static async UniTask<Scheme> LoadSchemeByGuid(string guidStr)
        {
            throw new NotImplementedException("Load scheme by guid is not implemented");
        }
        
    }
    

    public static class SchemesSaver
    {
        public static async UniTask<bool> SaveScheme(Scheme scheme)
        {
            throw new NotImplementedException("Save scheme is not implemented");
        }

        public static async UniTask<bool> SaveSchemes(Scheme[] schemes)
        {
            throw new NotImplementedException("Save schemes is not implemented");
        }
    }
}