using System;

namespace Overflow.Extensibility
{
    /// <summary>
    /// A base class for defining behavior registration attributes. The 
    /// OperationBehaviorAttributeFactory looks for implementations of this attribute
    /// on operations, using them to apply behavior instances to the operation.
    /// 
    /// If sufficient, this is the best way to attach behaviors since it avoids the
    /// need to create a custom behavior factory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class OperationBehaviorAttribute : Attribute
    {
        /// <summary>
        /// Create the desired behavior instance. This behavior will always be
        /// applied for operations having defiend the attribute.
        /// </summary>
        /// <returns>The behavior instance, uninitialized</returns>
        public abstract OperationBehavior CreateBehavior();
    }
}
