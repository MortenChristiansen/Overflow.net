using System;
using System.Collections.Generic;
using Overflow.Utilities;

namespace Overflow
{
    /// <summary>
    /// A base class for custom operations. Inherit from the type to implement
    /// the business operations of your workflows.
    /// </summary>
    public abstract class Operation : IOperation
    {
        private OperationContext _context;
        private WorkflowConfiguration _configuration;
        private readonly IList<ExecutionInfo>  _executedChildOperations = new List<ExecutionInfo>();

        protected abstract void OnExecute();

        public IEnumerable<ExecutionInfo> ExecutedChildOperations { get { return _executedChildOperations; }}

        public virtual void Initialize(WorkflowConfiguration configuration)
        {
            Verify.NotNull(configuration, "configuration");

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

            var started = Time.OffsetUtcNow;
            try
            {
                childOperation.Execute();
                _executedChildOperations.Add(new ExecutionInfo(childOperation, null, started, Time.OffsetUtcNow));
                
            }
            catch (Exception e)
            {
                _executedChildOperations.Add(new ExecutionInfo(childOperation, e, started, Time.OffsetUtcNow));
                throw;
            }
        }

        public virtual IEnumerable<IOperation> GetChildOperations()
        {
            return new IOperation[0];
        }

        public static IOperation Create<TOperation>(WorkflowConfiguration configuration)
            where TOperation : IOperation
        {
            Verify.Operation(configuration != null, "Operation.Configuration was not set.");
            Verify.Operation(configuration.Resolver != null, "Operation.Configuration.Resolver was not set. You can set it to a SimpleOperationResolver instance or add a more full featured, external implementation.");

            var operation = configuration.Resolver.Resolve<TOperation>(configuration);
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
