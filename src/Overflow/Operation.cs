using System.Collections.Generic;

namespace Overflow
{
    public abstract class Operation : IOperation
    {
        public static IOperationResolver Resolver { get; set; }

        protected abstract void OnExecute();

        public void Execute()
        {
            OnExecute();

            foreach (var childOperation in GetChildOperations())
                childOperation.Execute();
        }

        public virtual IEnumerable<IOperation> GetChildOperations()
        {
            return new IOperation[0];
        }
    }
}
