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

        /// <summary>
        /// Execute the business logic of the operation.
        /// </summary>
        protected abstract void OnExecute();

        /// <summary>
        /// Get the child operations which have been executed during the execution of this
        /// operation.
        /// </summary>
        public IEnumerable<ExecutionInfo> ExecutedChildOperations { get { return _executedChildOperations; }}

        /// <summary>
        /// Initialize the operation. 
        /// </summary>
        /// <param name="configuration">The global configuration of the workflow</param>
        public virtual void Initialize(WorkflowConfiguration configuration)
        {
            Verify.NotNull(configuration, "configuration");

            _configuration = configuration;
        }

        /// <summary>
        /// Run the logic of the operation followed by all the
        /// child operations.
        /// </summary>
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

        /// <summary>
        /// Yield the child operations of the operation. This collection should always be
        /// evaluated one at a time, executing each task before retreiving the next. Otherwise,
        /// correct execution cannot be expected.
        /// </summary>
        /// <returns>Any child operations. Note that the execution of each child operation can
        /// influence the next sibling operations.</returns>
        public virtual IEnumerable<IOperation> GetChildOperations()
        {
            return new IOperation[0];
        }

        /// <summary>
        /// Create a new initialized instance of an operation type.
        /// </summary>
        /// <typeparam name="TOperation">The type of operation to create</typeparam>
        /// <param name="configuration">The current workflow configuration</param>
        /// <returns>A new operation instance</returns>
        public static IOperation Create<TOperation>(WorkflowConfiguration configuration)
            where TOperation : IOperation
        {
            Verify.Operation(configuration != null, "Operation.Configuration was not set.");
            Verify.Operation(configuration.Resolver != null, "Operation.Configuration.Resolver was not set. You can set it to a SimpleOperationResolver instance or add a more full featured, external implementation.");

            var operation = configuration.Resolver.Resolve<TOperation>(configuration);
            operation.Initialize(configuration);
            return operation;
        }

        /// <summary>
        /// Create a new initialized instance of an operation type.
        /// </summary>
        /// <typeparam name="TOperation">The type of operation to create</typeparam>
        /// <returns>A new operation instance</returns>
        protected IOperation Create<TOperation>()
            where TOperation : IOperation
        {
            return Create<TOperation>(_configuration);
        }

        /// <summary>
        /// Create a new initialized instance of an input operation type.
        /// The operation is supplied with the specified input value.
        /// </summary>
        /// <typeparam name="TOperation">The type of operation to create</typeparam>
        /// <typeparam name="TInput">The type of input to provide it</typeparam>
        /// <param name="input">The input data</param>
        /// <returns>A new operation instance</returns>
        protected IOperation Create<TOperation, TInput>(TInput input)
            where TInput : class
            where TOperation : IOperation, IInputOperation<TInput>
        {
            var operation = Create<TOperation>(_configuration);

            ((IInputOperation<TInput>)operation).Input(input);

            return operation;
        }

        /// <summary>
        /// Find a value of a given type which has been produced as 
        /// output by a child operation implementing the IOutputOperation
        /// interface.
        /// </summary>
        /// <typeparam name="TOutput">The type of the value</typeparam>
        /// <returns>The value if it has been produced and null otherwise.</returns>
        protected TOutput GetChildOutputValue<TOutput>()
            where TOutput : class
        {
            return _context.GetOutput<TOutput>();
        }

        /// <summary>
        /// Find a collection of a given type which has been produced as 
        /// output by a child operation implementing the IOutputOperation
        /// interface. This method is a shorthand for 
        /// GetChildOutputValue&lt;IEnumerable&lt;TOutput&gt;&gt;.
        /// </summary>
        /// <typeparam name="TOutput">The type of the value</typeparam>
        /// <returns>The a collection of values if it has been produced and 
        /// null otherwise.</returns>
        protected IEnumerable<TOutput> GetChildOutputValues<TOutput>()
            where TOutput : class
        {
            return GetChildOutputValue<IEnumerable<TOutput>>();
        }
    }
}
