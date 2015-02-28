using System;

namespace Overflow
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class OperationBehaviorAttribute : Attribute
    {
        public abstract IOperation AddBehavior(IOperation operation);
    }
}
