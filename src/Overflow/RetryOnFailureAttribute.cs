using System;

namespace Overflow
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RetryOnFailureAttribute : Attribute { }
}
