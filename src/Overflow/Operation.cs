namespace Overflow
{
    public abstract class Operation : IOperation
    {
        protected abstract void OnExecute();

        public void Execute()
        {
            OnExecute();
        }
    }
}
