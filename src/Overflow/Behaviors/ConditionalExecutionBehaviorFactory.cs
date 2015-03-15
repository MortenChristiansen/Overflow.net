using System.Collections.Generic;
using Overflow.Extensibility;

namespace Overflow.Behaviors
{
    class ConditionalExecutionBehaviorFactory : IOperationBehaviorFactory
    {
        public IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration)
        {
            if (!(operation is IConditionalOperation))
                return new OperationBehavior[0];

            return new OperationBehavior[] { new ConditionalExecutionBehavior() };
        }
    }
}
