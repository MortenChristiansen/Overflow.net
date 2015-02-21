using System.Collections.Generic;

namespace Overflow
{
    public abstract class Operation : IOperation
    {
        private static IOperationResolver _resolver;
        public static IOperationResolver Resolver
        {
            get { return _resolver ?? (_resolver = new SimpleOperationResolver()); }
            set { _resolver = value; }
        }

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
