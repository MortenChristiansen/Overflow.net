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
            var registerHandlerMethod = outputOperationType.GetMethod("Output");

            var outputHandler = CreateOutputHandler(outputType);
            registerHandlerMethod.Invoke(operation, new object[] { outputHandler });
        }

        private Delegate CreateOutputHandler(Type type)
        {
            var method = typeof (OperationContext).GetMethod("OnOutput", BindingFlags.NonPublic | BindingFlags.Instance);
            var genericMethod = method.MakeGenericMethod(type);
            var actionT = typeof(Action<>).MakeGenericType(type);
            return Delegate.CreateDelegate(actionT, this, genericMethod);
        }

        // Used with generics
        private void OnOutput<TOutput>(TOutput output)
        {
            var key = typeof (TOutput);
            if (_values.ContainsKey(key))
                _values.Remove(key);

            _values.Add(key, output);
        }

        public void ProvideInputs(IOperation operation)
        {
            var innerOperation = operation.GetInnermostOperation();

            var inputOperationInterfaces = innerOperation.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IInputOperation<>));
            foreach (var inputOperationType in inputOperationInterfaces)
                ProvideInput(innerOperation, inputOperationType);
        }

        private void ProvideInput(IOperation operation, Type inputOperationType)
        {
            var inputType = inputOperationType.GetGenericArguments()[0];
            var provideInputMethod = inputOperationType.GetMethod("Input");

            var output = GetOutput(inputType);
            provideInputMethod.Invoke(operation, new []{ output });
            
            SaveValueForFutureChildOperationContexts(operation, inputType, output);
        }

        private object GetOutput(Type inputType)
        {
            if (!_values.ContainsKey(inputType)) return null;

            return _values[inputType];
        }

        private static void SaveValueForFutureChildOperationContexts(IOperation operation, Type inputType, object output)
        {
            var operationData = _operationData.GetOrCreateValue(operation);
            operationData.Add(inputType, output);
        }

        public TOutput GetOutput<TOutput>()
            where TOutput : class
        {
            return (TOutput)GetOutput(typeof(TOutput));
        }

        private static readonly ConditionalWeakTable<IOperation, Dictionary<Type, object>> _operationData = new ConditionalWeakTable<IOperation, Dictionary<Type, object>>();

        public static OperationContext Create(IOperation operation)
        {
            return new OperationContext(_operationData.GetOrCreateValue(operation));
        }
    }
}
