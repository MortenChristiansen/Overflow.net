using Overflow.Behaviors;

namespace Overflow
{
    /// <summary>
    /// A utility class for configuring workflows with standard behavior factories.
    /// </summary>
    public static class Workflow
    {
        /// <summary>
        /// Create a configuration with the standard behavior factories.
        /// </summary>
        /// <typeparam name="TOperation">The root operation type of the workflow</typeparam>
        /// <returns>The created configuration</returns>
        public static WorkflowConfiguration<TOperation> Configure<TOperation>() where TOperation : IOperation
        {
            return 
                new WorkflowConfiguration<TOperation> { Resolver = new SimpleOperationResolver() }.
                WithBehaviorFactory(new OperationBehaviorAttributeFactory()).
                WithBehaviorFactory(new OperationLoggingBehaviorFactory()).
                WithBehaviorFactory(new WorkflowRetryBehaviorFactory()).
                WithBehaviorFactory(new ConditionalExecutionBehaviorFactory())
                as WorkflowConfiguration<TOperation>;
        }
    }
}
