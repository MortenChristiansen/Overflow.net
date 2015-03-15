using Overflow.Behaviors;

namespace Overflow
{
    public static class Workflow
    {
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
