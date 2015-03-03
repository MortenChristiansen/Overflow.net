using System.Collections.Generic;

namespace Overflow.Test.Fakes
{
    class FakeOperationBehaviorFactory : IOperationBehaviorFactory
    {
        public IList<OperationBehavior> OperationBehaviors { get; private set; }

        public FakeOperationBehaviorFactory()
        {
            OperationBehaviors = new List<OperationBehavior>();
        }

        public IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration)
        {
            return OperationBehaviors;
        }
    }
}
