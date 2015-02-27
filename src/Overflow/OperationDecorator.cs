using System;
using System.Collections.Generic;

namespace Overflow
{
    public abstract class OperationDecorator : IOperation
    {
        internal IOperation DecoratedOperation { get; private set; }

        protected OperationDecorator(IOperation decoratedOperation)
        {
            if (decoratedOperation == null)
                throw new ArgumentNullException("decoratedOperation");

            DecoratedOperation = decoratedOperation;
        }


        public virtual void Execute()
        {
            DecoratedOperation.Execute();
        }

        public virtual IEnumerable<IOperation> GetChildOperations()
        {
            return DecoratedOperation.GetChildOperations();
        }
    }
}
