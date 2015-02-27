using System;
using System.Collections.Generic;

namespace Overflow
{
    public abstract class OperationDecorator : IOperation
    {
        public IOperation DecoratedOperation { get; private set; }

        protected OperationDecorator(IOperation decoratedOperation)
        {
            if (decoratedOperation == null)
                throw new NullReferenceException("decoratedOperation");

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
