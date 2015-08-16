using System.Collections.Generic;
using System.Linq;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow.Behaviors
{
    class WorkflowRetryBehaviorFactory : IOperationBehaviorFactory
    {
        public IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration)
        {
            Verify.NotNull(configuration, nameof(configuration));

            if (configuration.RetryExceptionTypes.Count == 0)
                return new OperationBehavior[0];

            return new OperationBehavior[] { new RetryBehavior(configuration.TimesToRetry, configuration.RetryDelay, configuration.RetryExceptionTypes.ToArray()) };
        }
    }
}
