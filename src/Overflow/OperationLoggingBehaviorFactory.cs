using System;
using System.Collections.Generic;

namespace Overflow
{
    class OperationLoggingBehaviorFactory : IOperationBehaviorFactory
    {
        public IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (configuration.Logger == null)
                return new OperationBehavior[0];

            return new [] { new OperationLoggingBehavior(configuration.Logger) };
        }
    }
}
