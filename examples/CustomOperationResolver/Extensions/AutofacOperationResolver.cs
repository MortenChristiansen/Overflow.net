using System.Linq;
using System.Reflection;
using Autofac;
using Overflow;
using Overflow.Extensibility;

namespace CustomOperationResolver.Extensions
{
    class AutofacOperationResolver : IOperationResolver
    {
        private readonly ILifetimeScope _scope;

        public AutofacOperationResolver(ILifetimeScope scope)
        {
            _scope = RegisterAllOperationTypes(scope);
        }

        private static ILifetimeScope RegisterAllOperationTypes(ILifetimeScope scope)
        {
            return scope.BeginLifetimeScope(builder =>
            {
                var assembly = Assembly.GetEntryAssembly();
                var operationTypes = assembly.GetTypes().Where(t => typeof(Operation).IsAssignableFrom(t));
                builder.RegisterTypes(operationTypes.ToArray());
            });
        }

        public IOperation Resolve<TOperation>(WorkflowConfiguration configuration) where TOperation : IOperation
        {
            var operation = _scope.Resolve<TOperation>();

            return OperationResolverHelper.ApplyBehaviors(operation, configuration);
        }
    }
}
