using System;
using System.Collections.Generic;

namespace Overflow
{
    public abstract class OperationBehavior : IOperation
    {
        internal IOperation InnerOperation { get; private set; }

        protected OperationBehavior(IOperation innerOperation)
        {
            if (innerOperation == null)
                throw new ArgumentNullException("innerOperation");

            InnerOperation = innerOperation;
        }

        public void Initialize(WorkflowConfiguration configuration)
        {
            InnerOperation.Initialize(configuration);
        }

        public virtual void Execute()
        {
            InnerOperation.Execute();
        }

        public virtual IEnumerable<IOperation> GetChildOperations()
        {
            return InnerOperation.GetChildOperations();
        }
    }
}
