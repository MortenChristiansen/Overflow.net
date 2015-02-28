namespace Overflow
{
    public static class Workflow
    {
        public static WorkflowConfiguration<TOperation> Configure<TOperation>() where TOperation : IOperation
        {
            return 
                new WorkflowConfiguration<TOperation> { Resolver = new SimpleOperationResolver() }.
                WithBehaviorBuilder(new OperationBehaviorAttributeBuilder())
                as WorkflowConfiguration<TOperation>;
        }
    }
}
