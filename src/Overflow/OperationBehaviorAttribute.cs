using System;

namespace Overflow
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class OperationBehaviorAttribute : Attribute
    {
        public Type OperationType { get; private set; }

        protected OperationBehaviorAttribute(Type operationType)
        {
            if (operationType == null)
                throw new ArgumentNullException("operationType");

            OperationType = operationType;
        }
    }
}
