using System;

namespace Overflow
{
    /// <summary>
    /// Marks the operation as being indempotent. This provides information to
    /// behaviors regarding whether it can be executed more than once.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IndempotentAttribute : Attribute
    {
    }
}
