using System;
using System.Collections.Generic;

namespace Overflow
{
    public abstract class Operation : IOperation
    {
        private OperationContext _context;

        public WorkflowConfiguration Configuration { get; set; }

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

        public static IOperation Create<TOperation>(WorkflowConfiguration configuration)
            where TOperation : IOperation
        {
            if (configuration == null)
                throw new InvalidOperationException("Operation.Configuration was not set.");

            if (configuration.Resolver == null)
                throw new InvalidOperationException("Operation.Configuration.Resolver was not set. You can set it to a SimpleOperationResolver instance or add a more full featured, external implementation.");

            return configuration.Resolver.Resolve<TOperation>();
        }

        protected IOperation Create<TOperation>()
            where TOperation : IOperation
        {
            return Create<TOperation>(Configuration);
        }

        protected TOutput GetChildOutputValue<TOutput>()
            where TOutput : class
        {
            return _context.GetOutput<TOutput>();
        }
    }
}
