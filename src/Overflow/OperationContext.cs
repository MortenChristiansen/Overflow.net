using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Overflow.Utilities;

namespace Overflow
{
    class OperationContext
    {
        private readonly Dictionary<Type, object> _values;

        private OperationContext(Dictionary<Type, object> values)
        {
            _values = values;
        }

        public void ProvideInputs(IOperation operation)
        {
            var innerOperation = operation.GetInnermostOperation();
            var innerOperationType = innerOperation.GetType();

            var inputPropertyAttributes = innerOperationType.GetProperties().Where(p => p.GetCustomAttributes(typeof(InputAttribute), true).Any()).Select(p => Tuple.Create(p, p.GetCustomAttributes(typeof(PipeAttribute), true).Any()));
            foreach (var inputProperty in inputPropertyAttributes)
                GetInput(innerOperation, inputProperty.Item1, inputProperty.Item2);
        }

        private void GetInput(IOperation operation, PropertyInfo inputOperation, bool pipeInputToChildOperation)
        {
            var output = GetOutput(inputOperation.PropertyType);
            if (output != null)
            {
                inputOperation.SetValue(operation, output, null);
                SaveValueForFutureChildOperationContexts(operation, inputOperation.PropertyType, output);

                if (pipeInputToChildOperation)
                    (operation as Operation)?.InternalPipeInputToChildOperations(output);
            }
        }

        private object GetOutput(Type inputType, bool allowSpecializedClasses = false)
        {
            if (allowSpecializedClasses)
                return _values.FirstOrDefault(v => inputType.IsAssignableFrom(v.Key)).Value;

            return !_values.ContainsKey(inputType) ? null : _values[inputType];
        }

        private static void SaveValueForFutureChildOperationContexts(IOperation operation, Type inputType, object output)
        {
            var operationData = _operationData.GetOrCreateValue(operation);

            if (operationData.ContainsKey(inputType))
                operationData.Remove(inputType);

            operationData.Add(inputType, output);
        }

        public TOutput GetOutput<TOutput>(bool allowSpecializedClasses = false)
            where TOutput : class
        {
            return (TOutput)GetOutput(typeof(TOutput), allowSpecializedClasses);
        }

        private static readonly ConditionalWeakTable<IOperation, Dictionary<Type, object>> _operationData = new ConditionalWeakTable<IOperation, Dictionary<Type, object>>();

        public static OperationContext Create(IOperation operation) =>
            new OperationContext(_operationData.GetOrCreateValue(operation));

        public void AddData<TData>(TData data)
        {
            var key = typeof(TData);
            AddData(key, data);
        }

        private void AddData(Type key, object data)
        {
            if (_values.ContainsKey(key))
                _values.Remove(key);

            _values.Add(key, data);
        }

        /// <summary>
        /// Adds the value of any property with the OutputAttribute to the context.
        /// </summary>
        /// <param name="childOperation">The operation to parse for properties.</param>
        /// <param name="parentOperation">The parent operation for potentially piping output values.</param>
        public void AddOutput(IOperation childOperation, IOperation parentOperation)
        {
            Verify.NotNull(parentOperation, nameof(parentOperation));

            var innerOperation = childOperation.GetInnermostOperation();
            var innerOperationType = innerOperation.GetType();
            var innerParentOperation = parentOperation.GetInnermostOperation();

            var pipedOutputProperties = innerParentOperation.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(OutputAttribute), true).Any() && p.GetCustomAttributes(typeof(PipeAttribute), true).Any()).ToList();

            var outputPropertyAttributes = innerOperationType.GetProperties().Where(p => p.GetCustomAttributes(typeof(OutputAttribute), true).Any());
            foreach (var outputProperty in outputPropertyAttributes)
            {
                var value = outputProperty.GetValue(innerOperation, null);
                AddData(outputProperty.PropertyType, value);

                var pipedOutputProperty = pipedOutputProperties.Find(p => p.PropertyType == outputProperty.PropertyType);
                pipedOutputProperty?.SetValue(innerParentOperation, value, null);
            }
        }
    }
}
