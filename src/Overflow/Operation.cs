using System;
using System.Collections.Generic;

namespace Overflow
{
    public abstract class Operation : IOperation
    {
        private OperationContext _context;

        public static IOperationResolver Resolver { get; set; }

        protected abstract void OnExecute();

        public void Execute()
        {
            _context = OperationContext.Create(this);

            OnExecute();

            foreach (var childOperation in GetChildOperations())
                ExecuteWithDataFlow(childOperation);
        }

        private void ExecuteWithDataFlow(IOperation childOperation)
        {
            _context.RegisterOutputHandlers(childOperation);
            _context.ProvideInputs(childOperation);

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

        protected TOutput GetChildOutputValue<TOutput>()
            where TOutput : class
        {
            return _context.GetOutput<TOutput>();
        }
    }
}
