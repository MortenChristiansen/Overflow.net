using System;
using System.Collections.Generic;
using Overflow.Utilities;
using System.Linq;

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
        protected virtual void OnExecute() { }

        /// <summary>
        /// Get the child operations which have been executed during the execution of this
        /// operation.
        /// </summary>
        public IEnumerable<ExecutionInfo> ExecutedChildOperations => _executedChildOperations;

        /// <summary>
        /// Initialize the operation. 
        /// </summary>
        /// <param name="configuration">The global configuration of the workflow</param>
        public virtual void Initialize(WorkflowConfiguration configuration)
        {
            Verify.NotNull(configuration, nameof(configuration));

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
                _context.AddOutput(childOperation);
                RegisterExecutedChildOperation(null, started, childOperation);
                
            }
            catch (Exception e)
            when (RegisterExecutedChildOperation(e, started, childOperation)) { }
        }

        private bool RegisterExecutedChildOperation(Exception e, DateTimeOffset started, IOperation childOperation)
        {
            _executedChildOperations.Add(new ExecutionInfo(childOperation, e, started, Time.OffsetUtcNow));
            return false;
        }

        /// <summary>
        /// Yield the child operations of the operation. This collection should always be
        /// evaluated one at a time, executing each task before retreiving the next. Otherwise,
        /// correct execution cannot be expected.
        /// </summary>
        /// <returns>Any child operations. Note that the execution of each child operation can
        /// influence the next sibling operations.</returns>
        public virtual IEnumerable<IOperation> GetChildOperations() =>
            new IOperation[0];

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

            try
            {
                var operation = configuration.Resolver.Resolve<TOperation>(configuration);
                operation.Initialize(configuration);
                return operation;
            }
            catch (Exception e)
            {
                throw new OperationCreationException<TOperation>(e);
            }
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
            where TOperation : IOperation
        {
            var operation = Create<TOperation>(_configuration);

            ProvideInput<TInput, TOperation>(operation.GetInnermostOperation(), input);

            return operation;
        }

        private void ProvideInput<TInput, TOperation>(IOperation operation, TInput input)
            where TInput : class
            where TOperation : IOperation
        {
            if (operation is IInputOperation<TInput>)
            {
                ((IInputOperation<TInput>)operation.GetInnermostOperation()).Input(input);
            }
            else
            {
                var inputProperty = typeof(TOperation).GetProperties().FirstOrDefault(p => p.PropertyType == typeof(TInput) && p.GetCustomAttributes(typeof(InputAttribute), true).Any());
                if (inputProperty != null)
                    inputProperty.SetValue(operation, input, null);
                else
                    throw new ArgumentException($"The operation type {typeof(TOperation).Name} does not have a public property of type {typeof(TInput).Name} with the {nameof(InputAttribute)}.");
            }
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
            Verify.Operation(_context != null, $"The {nameof(GetChildOutputValue)} method can only be called during execution");

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

        /// <summary>
        /// Makes the input available to child operations implementing the
        /// IInputOperation&lt;TInput&gt; interface.
        /// </summary>
        /// <param name="input">The input data which should be available to child operations</param>
        protected void PipeInputToChildOperations<TInput>(TInput input)
        {
            Verify.Operation(_context != null, $"The {nameof(PipeInputToChildOperations)} method can only be called during execution");

            _context.AddData(input);
        }
    }
}
