namespace Overflow.Test.Fakes
{
    class FakeOperationDecorator : OperationDecorator
    {
        public FakeOperationDecorator(IOperation decoratedOperation) : base(decoratedOperation) { }
    }
}
