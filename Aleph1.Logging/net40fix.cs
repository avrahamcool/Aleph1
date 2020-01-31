#if NET40
namespace System.Runtime.CompilerServices
{
    /// <summary></summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerMemberNameAttribute : Attribute
    {
    }
    
    /// <summary></summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerFilePathAttribute : Attribute
    {
    }

    /// <summary></summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerLineNumberAttribute : Attribute
    {
    }
}
#endif