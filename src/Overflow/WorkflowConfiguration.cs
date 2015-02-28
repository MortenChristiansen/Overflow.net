namespace Overflow
{
    public class WorkflowConfiguration<TOperation> : WorkflowConfiguration
        where TOperation : IOperation
    {
        public IOperation CreateOperation()
        {
            return (TOperation)new SimpleOperationResolver().Resolve<TOperation>();
        }

    }

    public class WorkflowConfiguration
    {
        public IOperationResolver Resolver { get; set; }
    }
}
