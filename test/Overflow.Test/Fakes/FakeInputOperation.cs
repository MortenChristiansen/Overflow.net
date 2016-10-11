namespace Overflow.Test.Fakes
{
    public class FakeInputOperation<TInput> : FakeOperation, IInputOperation<TInput> where TInput : class
    {
        [Input] public TInput ProvidedInput { get; set; }
        public bool InputWasProvided { get; private set; }

        public FakeInputOperation(params IOperation[] childOperations)
            : base(childOperations)
        { }

        public void Input(TInput input)
        {
            ProvidedInput = input;
            InputWasProvided = true;
        }
    }
}
