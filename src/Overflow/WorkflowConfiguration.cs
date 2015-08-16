using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Behaviors;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow
{
    /// <summary>
    /// A workflow configuration capable of instantiating an operation
    /// of a specific type.
    /// </summary>
    /// <typeparam name="TOperation">The root operation type for the configured workflow</typeparam>
    public class WorkflowConfiguration<TOperation> : WorkflowConfiguration
        where TOperation : IOperation
    {
        /// <summary>
        /// Create a new operation instance with this configuration with
        /// the Continue On Failure behavior.
        /// </summary>
        /// <returns>The configurad operation</returns>
        public override IOperation CreateOperation()
        {
            return new ContinueOnFailureBehavior().AttachTo(Operation.Create<TOperation>(this));
        }
    }

    /// <summary>
    /// A configuration containing global settings for the execution of
    /// a workflow.
    /// </summary>
    public abstract class WorkflowConfiguration
    {
        /// <summary>
        /// The operation resolver used for instantiating operations when calling the Operation.Create method.
        /// </summary>
        public IOperationResolver Resolver { get; set; }
        /// <summary>
        /// The logger using to log operation executions.
        /// </summary>
        public IWorkflowLogger Logger { get; set; }
        /// <summary>
        /// The registered behavior factories. They define which behaviors can be used in the
        /// workflow.
        /// </summary>
        public IList<IOperationBehaviorFactory> BehaviorFactories { get; private set; }
        /// <summary>
        /// Global rules for retrying operations. Can be overwritten in
        /// the individual Retry attributes.
        /// </summary>
        public IList<Type> RetryExceptionTypes { get; private set; }
        /// <summary>
        /// Global rules for retrying operations. Can be overwritten in
        /// the individual Retry attributes.
        /// </summary>
        public int TimesToRetry { get; set; }
        /// <summary>
        /// Global rules for retrying operations. Can be overwritten in
        /// the individual Retry attributes.
        /// </summary>
        public TimeSpan RetryDelay { get; set; }

        /// <summary>
        /// Create a new operation.
        /// </summary>
        /// <returns>A new operation based on configuration.</returns>
        public abstract IOperation CreateOperation();

        /// <summary>
        /// Create a new configuration-
        /// </summary>
        protected WorkflowConfiguration()
        {
            BehaviorFactories = new List<IOperationBehaviorFactory>();
            RetryExceptionTypes = new List<Type>();
        }

        /// <summary>
        /// Fluent API for setting the operation resolver.
        /// </summary>
        /// <param name="resolver">The operation resolver to set</param>
        /// <returns>The updated configuration</returns>
        public WorkflowConfiguration WithResolver(IOperationResolver resolver)
        {
            Resolver = resolver;

            return this;
        }

        /// <summary>
        /// Fluent API for setting the workflow logger.
        /// </summary>
        /// <param name="logger">The logger to set</param>
        /// <returns>The updated configuration</returns>
        public WorkflowConfiguration WithLogger(IWorkflowLogger logger)
        {
            Logger = logger;

            return this;
        }

        /// <summary>
        /// Fluent API for adding an operation behavior factory.
        /// </summary>
        /// <param name="factory">The factory to add</param>
        /// <returns>The updated configuration</returns>
        public WorkflowConfiguration WithBehaviorFactory(IOperationBehaviorFactory factory)
        {
            Verify.NotNull(factory, nameof(factory));

            BehaviorFactories.Add(factory);

            return this;
        }

        /// <summary>
        /// Fluent API for setting the default retry settings.
        /// </summary>
        /// <param name="timesToRetry">The default number of times to retry an operation</param>
        /// <param name="retryDelay">The default delay in milliseconds before retrying an operation</param>
        /// <param name="retryExceptionTypes">The default exceptions to retry</param>
        /// <returns>The updated configuration</returns>
        public WorkflowConfiguration WithGlobalRetryBehavior(int timesToRetry, TimeSpan retryDelay, params Type[] retryExceptionTypes)
        {
            Verify.Argument(retryExceptionTypes.All(t => typeof (Exception).IsAssignableFrom(t)), "Only exception types are valid.");

            TimesToRetry = timesToRetry;
            RetryDelay = retryDelay;

            foreach(var retryExceptionType in retryExceptionTypes)
                RetryExceptionTypes.Add(retryExceptionType);

            return this;
        }
    }
}
