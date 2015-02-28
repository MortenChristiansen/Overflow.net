namespace Overflow.Test.Fakes
{
    class FakeOperationBehaviorBuilder : IOperationBehaviorBuilder
    {
        public IOperation ApplyBehavior(IOperation operation, WorkflowConfiguration configuration)
        {
            return new FakeOperationBehavior(operation);
        }
    }
}
