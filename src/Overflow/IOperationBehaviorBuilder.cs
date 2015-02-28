namespace Overflow
{
    public interface IOperationBehaviorBuilder
    {
        IOperation ApplyBehavior(IOperation operation, WorkflowConfiguration configuration);
    }
}
