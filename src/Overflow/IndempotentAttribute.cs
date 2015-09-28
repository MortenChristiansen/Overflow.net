using System;

namespace Overflow
{
    /// <summary>
    /// Marks the operation as being idempotent. This provides information to
    /// behaviors regarding whether it can be executed more than once.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [Obsolete("The attribute was renamed to fix a typo. Use IdempotentAttribute instead. This class will be removed in verson 2.0.")]
    public class IndempotentAttribute : IdempotentAttribute
    {
    }
}
