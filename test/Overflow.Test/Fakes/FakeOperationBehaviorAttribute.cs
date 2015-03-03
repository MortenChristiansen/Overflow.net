namespace Overflow.Test.Fakes
{
    class FakeOperationBehaviorAttribute : OperationBehaviorAttribute
    {
        public override OperationBehavior CreateBehavior()
        {
            return new FakeOperationBehavior();
        }
    }
}
