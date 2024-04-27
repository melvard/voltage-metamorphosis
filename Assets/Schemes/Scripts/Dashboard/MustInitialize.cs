namespace Schemes.Dashboard
{
    public abstract class MustInitialize<T>
    {
        protected MustInitialize(T parameter){}
    }
    
    public abstract class MustInitialize<T1, T2>
    {
        protected MustInitialize(T1 param1, T2 param2){}
    }
    
    public abstract class MustInitialize<T1, T2, T3>
    {
        protected MustInitialize(T1 param1, T2 param2, T3 param3){}
    }
}