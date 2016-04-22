using System;
using System.Collections.Generic;
using Overflow.Extensibility;

namespace Overflow.Testing
{
    /// <summary>
    /// An implementation of the IOperationResolver interface which allows the resolved child operations
    /// of an operation to be replaced with fake implementations. The fake implementations should inherit
    /// from the implementations which the parent operation would normally resolve, only the relevant virtual 
    /// methods being overridden as needed.
    /// </summary>
    public class FakeOperationResolver : IOperationResolver
    {
        private readonly bool _applyBehaviors;
        private readonly IOperationResolver _innerResolver;
        private readonly Dictionary<Type, IOperation> _typeMappings = new Dictionary<Type, IOperation>();

        /// <summary>
        /// Create a new FakeOperationResolver instance. 
        /// </summary>
        /// <param name="innerOperationResolver">An implementation of the IOperationResolver which the resolver
        /// falls back to whenever a requested operation type does not have a fake instance registered. Defaults
        /// to the SimpleOperationResolver class.</param>
        /// <param name="applyBehaviors">Whether to apply behaviors to fake operations before returning them.
        /// Since the behaviors are seldom relevant for testing, this is turned off by default. Behaviors are
        /// still applied to operations with no fakes registered for them.</param>
        public FakeOperationResolver(IOperationResolver innerOperationResolver = null, bool applyBehaviors = false)
        {
            _applyBehaviors = applyBehaviors;
            _innerResolver = innerOperationResolver ?? new SimpleOperationResolver();
        }

        /// <summary>
        /// Register an operation to be returned when the parent operation asks
        /// for a specific type of operation to be resolved.
        /// </summary>
        /// <typeparam name="TOperation">The type of the operation that the parent operation
        /// needs.</typeparam>
        /// <param name="fakeImplementation">An operation that inherits from the TOperation class
        /// without being of that specific class.</param>
        public void ProvideFakeOperation<TOperation>(TOperation fakeImplementation) where TOperation : IOperation
        {
            var requestType = typeof(TOperation);
            var implementationType = fakeImplementation.GetType();

            if (requestType == implementationType)
                throw new ArgumentException("Original and implementation types cannot be the same type");

            if (_typeMappings.ContainsKey(requestType))
                _typeMappings[requestType] = fakeImplementation;
            else
                _typeMappings.Add(requestType, fakeImplementation);
        }

        /// <summary>
        /// Create a new instance of an operation. All behaviors are correctly attached
        /// and initialized. Constructor arguments are supplied.
        /// 
        /// In case an operation is requested where a fake implementation has been supplied,
        /// the fake instance is returned instead. Whether behaviors are applied to this
        /// instance is configured using the applyBehaviors constructor argument of this
        /// class.
        /// </summary>
        /// <typeparam name="TOperation">The IOperation implementation to create</typeparam>
        /// <param name="configuration">The global workflow configuration</param>
        /// <returns>The newly created operation instance</returns>
        public IOperation Resolve<TOperation>(WorkflowConfiguration configuration) where TOperation : IOperation
        {
            var requestType = typeof(TOperation);
            if (!_typeMappings.ContainsKey(requestType))
                return _innerResolver.Resolve<TOperation>(configuration);

            var operation= _typeMappings[requestType];
            return _applyBehaviors ? OperationResolverHelper.ApplyBehaviors(operation, configuration) : operation;
        }
    }
}
