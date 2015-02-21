using System;
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

        public static IOperation Create<TOperation>()
            where TOperation : IOperation
        {
            if (Resolver == null)
                throw new InvalidOperationException("Operation.Resolver was not set. You can set it to a SimpleOperationResolver instance or add a more full featured, external implementation.");

            return Resolver.Resolve<TOperation>();
        }
    }
}
