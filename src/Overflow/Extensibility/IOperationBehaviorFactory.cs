using System.Collections.Generic;

namespace Overflow.Extensibility
{
    public interface IOperationBehaviorFactory
    {
        IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration);
    }
}
