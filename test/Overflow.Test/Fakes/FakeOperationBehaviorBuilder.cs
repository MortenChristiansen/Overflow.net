namespace Overflow.Test.Fakes
{
    class FakeOperationBehaviorBuilder : IOperationBehaviorBuilder
    {
        public IOperation ApplyBehavior(IOperation operation)
        {
            return new FakeOperationBehavior(operation);
        }
    }
}
