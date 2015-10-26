using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Overflow
{
    class OperationContext
    {
        private readonly Dictionary<Type, object> _values;

        private OperationContext(Dictionary<Type, object> values)
        {
            _values = values;
        }

        public void RegisterOutputHandlers(IOperation operation)
        {
            var innerOperation = operation.GetInnermostOperation();

            var outputOperationInterfaces = innerOperation.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOutputOperation<>));
            foreach (var outputOperationType in outputOperationInterfaces)
                RegisterOutputHandler(innerOperation, outputOperationType);
        }

        private void RegisterOutputHandler(IOperation operation, Type outputOperationType)
        {
            var outputType = outputOperationType.GetGenericArguments()[0];
            var registerHandlerMethod = outputOperationType.GetMethod(nameof(IOutputOperation<object>.Output));

            var outputHandler = CreateOutputHandler(outputType);
            registerHandlerMethod.Invoke(operation, new object[] { outputHandler });
        }

        private Delegate CreateOutputHandler(Type type)
        {
            var method = typeof (OperationContext).GetMethod(nameof(OnOutput), BindingFlags.NonPublic | BindingFlags.Instance);
            var genericMethod = method.MakeGenericMethod(type);
            var actionT = typeof(Action<>).MakeGenericType(type);
            return Delegate.CreateDelegate(actionT, this, genericMethod);
        }

        private void OnOutput<TOutput>(TOutput output)
        {
            AddData(output);
        }

        public void ProvideInputs(IOperation operation)
        {
            var innerOperation = operation.GetInnermostOperation();
            var innerOperationType = innerOperation.GetType();

            var inputOperationInterfaces = innerOperationType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IInputOperation<>));
            foreach (var inputOperationType in inputOperationInterfaces)
                GetInput(innerOperation, inputOperationType);

            var inputPropertyAttributes = innerOperationType.GetProperties().Where(p => p.GetCustomAttributes(typeof(InputAttribute), true).Any()).Select(p => Tuple.Create(p, p.GetCustomAttributes(typeof(PipeAttribute), true).Any()));
            foreach (var inputProperty in inputPropertyAttributes)
                GetInput(innerOperation, inputProperty.Item1, inputProperty.Item2);
        }

        private void GetInput(IOperation operation, Type inputOperationType)
        {
            var inputType = inputOperationType.GetGenericArguments()[0];
            var provideInputMethod = inputOperationType.GetMethod(nameof(IInputOperation<object>.Input));

            var output = GetOutput(inputType);
            if (output != null)
            {
                provideInputMethod.Invoke(operation, new[] { output });
                SaveValueForFutureChildOperationContexts(operation, inputType, output);
            }
        }

        private void GetInput(IOperation operation, PropertyInfo inputOperation, bool pipeInputToChildOperation)
        {
            var output = GetOutput(inputOperation.PropertyType);
            if (output != null)
            {
                inputOperation.SetValue(operation, output, null);
                SaveValueForFutureChildOperationContexts(operation, inputOperation.PropertyType, output);

                if (pipeInputToChildOperation && operation is Operation)
                {
                    var pipeMethod = typeof(Operation).GetMethod("PipeInputToChildOperations", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(inputOperation.PropertyType);
                    pipeMethod.Invoke(operation, new[] { output });
                }
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
        /// <param name="operation">The operation to parse for properties.</param>
        public void AddOutput(IOperation operation)
        {
            var innerOperation = operation.GetInnermostOperation();
            var innerOperationType = innerOperation.GetType();

            var outputPropertyAttributes = innerOperationType.GetProperties().Where(p => p.GetCustomAttributes(typeof(OutputAttribute), true).Any());
            foreach (var outputProperty in outputPropertyAttributes)
                AddData(outputProperty.PropertyType, outputProperty.GetValue(innerOperation, null));
        }
    }
}
