using System.Collections.Generic;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow.Behaviors
{
    class OperationLoggingBehaviorFactory : IOperationBehaviorFactory
    {
        public IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration)
        {
            Verify.NotNull(operation, "operation");
            Verify.NotNull(configuration, "configuration");

            if (configuration.Logger == null)
                return new OperationBehavior[0];

            return new OperationBehavior[]
            {
                new OperationExecutionLoggingBehavior(configuration.Logger),
                new OperationErrorLoggingBehavior(configuration.Logger)
            };
        }
    }
}
