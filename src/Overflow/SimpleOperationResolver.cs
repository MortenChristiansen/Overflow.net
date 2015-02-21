namespace Overflow
{
    class SimpleOperationResolver : IOperationResolver
    {
        public void RegisterOperationDependency<TDependency, TDependencyImplementation>()
        {
            throw new System.NotImplementedException();
        }

        public TOperation Resolve<TOperation>() where TOperation : IOperation
        {
            throw new System.NotImplementedException();
        }
    }
}
