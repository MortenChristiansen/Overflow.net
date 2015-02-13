namespace Overflow.Test.Fakes
{
    class FakeTask : Task
    {
        public bool HasExecuted { get; private set; }

        protected override void OnExecute()
        {
            HasExecuted = true;
        }
    }
}
