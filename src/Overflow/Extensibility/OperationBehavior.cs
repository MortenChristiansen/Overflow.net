using System.Collections.Generic;
using Overflow.Utilities;

namespace Overflow.Extensibility
{
    /// <summary>
    /// The base class for custom behaviors. Inheriting from this class
    /// and overriding the Initialize or GetChildOperations method allows
    /// you to inject the special behavior functionality. Use a 
    /// IOperationBehaviorFactory to define when the behaviors should be
    /// created.
    /// </summary>
    public abstract class OperationBehavior : IOperation
    {
        private IWorkflowLogger _logger;
        internal IOperation InnerOperation { get; private set; }

        /// <summary>
        /// The precedence level of the behavior. Determines the order in
        /// which all the behaviors for a given operation are to be applied.
        /// </summary>
        public abstract BehaviorPrecedence Precedence { get; }

        /// <summary>
        /// Attach the behavior to an operation. This behavior will 
        /// provide a decorator for the operation, making the behavior
        /// transparent to the client code.
        /// </summary>
        /// <param name="innerOperation">The operation which is to be
        /// decorated with additional behavior</param>
        /// <returns>Returns the behavior.</returns>
        public IOperation AttachTo(IOperation innerOperation)
        {
            Verify.NotNull(innerOperation, "innerOperation");

            InnerOperation = innerOperation;

            return this;
        }

        /// <summary>
        /// Gets the child operations which have been executed.
        /// </summary>
        public IEnumerable<ExecutionInfo> ExecutedChildOperations => InnerOperation.ExecutedChildOperations;

        /// <summary>
        /// Initialize the behavior and the decorated operation.
        /// </summary>
        /// <param name="configuration">The configruation for the current
        /// workflow</param>
        public void Initialize(WorkflowConfiguration configuration)
        {
            Verify.NotNull(configuration, "configuration");

            _logger = configuration.Logger;

            InnerOperation.Initialize(configuration);
        }

        /// <summary>
        /// Execute the behavior. The behavior has a chance to apply special logic before
        /// or after invoking the Execute method on the inner operation.
        /// </summary>
        public virtual void Execute() => 
            InnerOperation.Execute();

        /// <summary>
        /// Get child operations for the decorated operation. The behavior has a chance to
        /// perform special logic before or after retreiving the operations.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IOperation> GetChildOperations() =>  
            InnerOperation.GetChildOperations();

        /// <summary>
        /// Register the application of a behavior. Use this when the behavior causes the normal
        /// execution flow to be abandoned, documenting what happened. Behavior that has no impact
        /// on the execution should not be registered.
        /// </summary>
        /// <param name="description">A text describing how the workflow was modified by the behavior</param>
        protected void BehaviorWasApplied(string description)
        {
            Verify.NotNull(description, "description");

            _logger?.BehaviorWasApplied(InnerOperation.GetInnermostOperation(), this, description);
        }
    }
}
