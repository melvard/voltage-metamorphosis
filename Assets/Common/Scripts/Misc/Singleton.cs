namespace Common.Misc
{
    public class Singleton<T> where T : class, new()
    {
        public static T Instance => _instance ??= new T();
        private static T _instance;
    }
}