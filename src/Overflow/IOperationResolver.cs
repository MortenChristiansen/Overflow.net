namespace Overflow
{
    public interface IOperationResolver
    {
        IOperation Resolve<TOperation>(WorkflowConfiguration configuration) where TOperation : IOperation;
    }
}
