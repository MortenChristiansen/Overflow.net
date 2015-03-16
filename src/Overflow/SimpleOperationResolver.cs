using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Utilities;

namespace Overflow
{
    class SimpleOperationResolver : IOperationResolver
    {
        private readonly IDictionary<Type, Type> _mappings = new Dictionary<Type, Type>(); 

        public void RegisterOperationDependency<TDependency, TDependencyImplementation>()
             where TDependencyImplementation : TDependency
        {
            var dependencyType = typeof (TDependency);
            if (_mappings.ContainsKey(dependencyType))
                _mappings.Remove(dependencyType);
            _mappings.Add(dependencyType, typeof(TDependencyImplementation));
        }

        public IOperation Resolve<TOperation>(WorkflowConfiguration configuration) where TOperation : IOperation
        {
            Verify.NotNull(configuration, "configuration");

            var actualOperation = GetActualOperation<TOperation>();
            return GetDecoratedOperation(actualOperation, configuration) ?? actualOperation;
        }

        private IOperation GetDecoratedOperation(IOperation innerOperation, WorkflowConfiguration configuration)
        {
            if (configuration.BehaviorFactories.Count == 0) return null;

            foreach (var behaviorFactory in configuration.BehaviorFactories)
            {
                var behaviors = behaviorFactory.CreateBehaviors(innerOperation, configuration);
                foreach (var behavior in behaviors.OrderByDescending(b => b.Precedence))
                    innerOperation = behavior.Attach(innerOperation);
            }

            return innerOperation;
        }

        private IOperation GetActualOperation<TOperation>() where TOperation : IOperation
        {
            var operationType = typeof (TOperation);
            var parameters = ResolveConstructorParameters(operationType);

            return (TOperation)Activator.CreateInstance(operationType, parameters);
        }

        private object[] ResolveConstructorParameters(Type type)
        {
            var constructors = type.GetConstructors();
            Verify.Operation(constructors.Length == 1, "Type " + type.Name + " has more than one constructor. This is not supported.");

            return constructors[0].GetParameters().Select(p => ResolveConstructorParameter(p.ParameterType)).ToArray();
        }

        private object ResolveConstructorParameter(Type parameterType)
        {
            Verify.Operation(_mappings.ContainsKey(parameterType), "Type " + parameterType.Name + " could not be resolved.");

            var implementationType = _mappings[parameterType];
            var parameters = ResolveConstructorParameters(implementationType);
            return Activator.CreateInstance(implementationType, parameters);

        }
    }
}
