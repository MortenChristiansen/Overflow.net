namespace Overflow.Test.Fakes
{
    public class FakeInputOperation<TInput> : FakeOperation where TInput : class
    {
        [Input] public TInput ProvidedInput { get; set; }
        public bool InputWasProvided => ProvidedInput != null;

        public FakeInputOperation(params IOperation[] childOperations)
            : base(childOperations)
        { }
    }
}
