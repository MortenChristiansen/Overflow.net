using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow
{
    /// <summary>
    /// A simple IOperationResolver implementation which allows you
    /// to register dependencies which can be supplied through the
    /// constructor when resolving operations.
    /// 
    /// This implementation does not perform any lifecycle management
    /// and only supports operations with a single constructor defined.
    /// 
    /// To support more complex dependency management, an implementation
    /// should be built based on an IoC container.
    /// </summary>
    public class SimpleOperationResolver : IOperationResolver
    {
        private readonly IDictionary<Type, Type> _mappings = new Dictionary<Type, Type>(); 

        /// <summary>
        /// Register a dependency type as being available to the resolved operations as a
        /// constructor argument.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency to supply</typeparam>
        /// <typeparam name="TDependencyImplementation">The implementing type</typeparam>
        public void RegisterOperationDependency<TDependency, TDependencyImplementation>()
            where TDependencyImplementation : class, TDependency
        {
            var dependencyType = typeof (TDependency);
            if (_mappings.ContainsKey(dependencyType))
                _mappings.Remove(dependencyType);
            _mappings.Add(dependencyType, typeof(TDependencyImplementation));
        }

        /// <summary>
        /// Resolve an operation instance. It can take any of the registered dependency
        /// types as constructor arguments.
        /// </summary>
        /// <typeparam name="TOperation">The type of operation to create</typeparam>
        /// <param name="configuration">The current workflow configuration</param>
        /// <returns>The new, uninitialized operation</returns>
        public IOperation Resolve<TOperation>(WorkflowConfiguration configuration) where TOperation : IOperation
        {
            Verify.NotNull(configuration, "configuration");

            var actualOperation = GetActualOperation<TOperation>();
            return OperationResolverHelper.ApplyBehaviors(actualOperation, configuration);
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
