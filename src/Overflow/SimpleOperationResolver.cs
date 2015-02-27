using System;
using System.Collections.Generic;
using System.Linq;

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

        public IOperation Resolve<TOperation>() where TOperation : IOperation
        {
            var actualOperation = GetActualOperation<TOperation>();

            if (typeof (TOperation).GetCustomAttributes(typeof (RetryOnFailureAttribute), inherit: false).Length > 0)
                return new RetryOnFailureOperationDecorator(actualOperation);

            return actualOperation;
        }

        private IOperation GetActualOperation<TOperation>() where TOperation : IOperation
        {
            var operationType = typeof (TOperation);
            var parameters = ResolveConstructorParameters(operationType);

            if (parameters == null)
                return default(TOperation);

            return (TOperation)Activator.CreateInstance(operationType, parameters);
        }

        private object[] ResolveConstructorParameters(Type type)
        {
            var constructors = type.GetConstructors();
            if (constructors.Length != 1)
                throw new InvalidOperationException("Type " + type.Name + " has more than one constructor. This is not supported.");
            return constructors[0].GetParameters().Select(p => ResolveConstructorParameter(p.ParameterType)).ToArray();
        }

        private object ResolveConstructorParameter(Type parameterType)
        {
            if (!_mappings.ContainsKey(parameterType))
                throw new InvalidOperationException("Type " + parameterType.Name + " could not be resolved.");

            var implementationType = _mappings[parameterType];
            var parameters = ResolveConstructorParameters(implementationType);
            return Activator.CreateInstance(implementationType, parameters);

        }
    }
}
