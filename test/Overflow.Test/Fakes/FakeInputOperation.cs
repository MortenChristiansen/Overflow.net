namespace Overflow.Test.Fakes
{
    class FakeInputOperation<TInput> : Operation, IInputOperation<TInput> where TInput : class
    {
        public TInput ProvidedInput { get; private set; }

        protected override void OnExecute() { }

        public void Input(TInput input)
        {
            ProvidedInput = input;
        }
    }
}
