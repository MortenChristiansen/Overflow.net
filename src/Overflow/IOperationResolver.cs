namespace Overflow
{
    public interface IOperationResolver
    {
        void RegisterOperationDependency<TDependency, TDependencyImplementation>();
        TOperation Resolve<TOperation>() where TOperation : IOperation;
    }
}
