namespace Overflow.Test.Fakes
{
    class FakeOperationBehavior : OperationBehavior
    {
        public FakeOperationBehavior(IOperation innerOperation) : base(innerOperation) { }
    }
}
