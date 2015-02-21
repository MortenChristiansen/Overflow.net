namespace Overflow.Test.Fakes
{
    class FakeOperation : Operation
    {
        public bool HasExecuted { get; private set; }

        protected override void OnExecute()
        {
            HasExecuted = true;
        }
    }
}
