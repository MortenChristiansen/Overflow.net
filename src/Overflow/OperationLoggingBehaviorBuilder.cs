using System;

namespace Overflow
{
    class OperationLoggingBehaviorBuilder : IOperationBehaviorBuilder
    {
        public IOperation ApplyBehavior(IOperation operation, WorkflowConfiguration configuration)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (configuration.Logger == null)
                return operation;

            return new OperationLoggingBehavior(operation, configuration.Logger);
        }
    }
}
