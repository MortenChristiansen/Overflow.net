namespace Overflow
{
    public interface IOperationResolver
    {
        void RegisterOperationDependency<TDependency, TDependencyImplementation>() where TDependencyImplementation : TDependency;
        TOperation Resolve<TOperation>() where TOperation : IOperation;
    }
}
