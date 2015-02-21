namespace Overflow.Test.Fakes
{
    class FakeOperationResolver : IOperationResolver
    {
        public void RegisterOperationDependency<TDependency, TDependencyImplementation>()
        {
            
        }

        public TOperation Resolve<TOperation>() where TOperation : IOperation
        {
            return default(TOperation);
        }
    }
}
