using Overflow.Extensibility;

namespace Overflow.Test.Fakes
{
    class FakeOperationBehaviorAttribute : OperationBehaviorAttribute
    {
        public override OperationBehavior CreateBehavior(WorkflowConfiguration configuration)
        {
            return new FakeOperationBehavior(configuration);
        }
    }
}
