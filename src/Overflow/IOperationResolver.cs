namespace Overflow
{
    public interface IOperationResolver
    {
        TOperation Resolve<TOperation>() where TOperation : IOperation;
    }
}
