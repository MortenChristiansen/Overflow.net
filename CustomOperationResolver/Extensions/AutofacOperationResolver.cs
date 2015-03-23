using Overflow;

namespace CustomOperationResolver.Extensions
{
    class AutofacOperationResolver : IOperationResolver
    {
        public IOperation Resolve<TOperation>(WorkflowConfiguration configuration) where TOperation : IOperation
        {
            throw new System.NotImplementedException();
        }
    }
}
