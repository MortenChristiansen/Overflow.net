namespace Overflow
{
    public class WorkflowConfiguration<TOperation> : WorkflowConfiguration
        where TOperation : IOperation
    {
        public IOperation CreateOperation()
        {
            return (TOperation)Operation.Create<TOperation>(this);
        }
    }

    public class WorkflowConfiguration
    {
        public IOperationResolver Resolver { get; set; }
        public IWorkflowLogger Logger { get; set; }

        public WorkflowConfiguration WithResolver(IOperationResolver resolver)
        {
            Resolver = resolver;

            return this;
        }

        public WorkflowConfiguration WithLogger(IWorkflowLogger logger)
        {
            Logger = logger;

            return this;
        }
    }
}
