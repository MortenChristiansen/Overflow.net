using System;
using System.Collections.Generic;

namespace Overflow
{
    public abstract class Operation : IOperation
    {
        private OperationContext _context;
        private WorkflowConfiguration _configuration;

        protected abstract void OnExecute();

        public virtual void Initialize(WorkflowConfiguration configuration)
        {
            _configuration = configuration;
        }

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

            var operation = configuration.Resolver.Resolve<TOperation>();
            operation.Initialize(configuration);
            return operation;
        }

        protected IOperation Create<TOperation>()
            where TOperation : IOperation
        {
            return Create<TOperation>(_configuration);
        }

        protected TOutput GetChildOutputValue<TOutput>()
            where TOutput : class
        {
            return _context.GetOutput<TOutput>();
        }
    }
}
