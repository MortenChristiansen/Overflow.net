namespace Overflow
{
    public abstract class Task : ITask
    {
        protected abstract void OnExecute();

        public void Execute()
        {
            OnExecute();
        }
    }
}
