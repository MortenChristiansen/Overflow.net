namespace Overflow.Test.Fakes
{
    class FakeOperationResolver : IOperationResolver
    {
        public void RegisterOperationDependency<TDependency, TDependencyImplementation>()
             where TDependencyImplementation : TDependency
        {
            
        }

        public TOperation Resolve<TOperation>() where TOperation : IOperation
        {
            return default(TOperation);
        }
    }
}
