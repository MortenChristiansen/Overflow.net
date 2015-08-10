using System.Collections.Generic;
using Overflow.Extensibility;

namespace Overflow.Test.Fakes
{
    class FakeOperationBehaviorFactory : IOperationBehaviorFactory
    {
        public IList<OperationBehavior> OperationBehaviors { get; } = new List<OperationBehavior>();

        public IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration) =>
            OperationBehaviors;
    }
}
