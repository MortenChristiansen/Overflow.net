namespace Overflow.Test.Fakes
{
    class FakeOperationBehaviorAttribute : OperationBehaviorAttribute
    {
        public override IOperation AddBehavior(IOperation operation)
        {
            return new FakeOperationBehavior(operation);
        }
    }
}
