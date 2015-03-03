using System.Collections.Generic;

namespace Overflow
{
    public interface IOperationBehaviorFactory
    {
        IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration);
    }
}
