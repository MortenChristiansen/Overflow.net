namespace Overflow
{
    public interface IOperationResolver
    {
        IOperation Resolve<TOperation>() where TOperation : IOperation;
    }
}
