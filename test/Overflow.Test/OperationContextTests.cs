using System;
using Xunit;

namespace Overflow.Test
{
    public class OperationContextTests
    {
        [Fact]
        public void Registering_output_handlers_calls_registration_methods_on_output_operation()
        {
            var sut = new OperationContext();
            var operation = new OutputOperation<object>();

            sut.RegisterOutputHandlers(operation);

            Assert.NotNull(operation.OnReceiveOutput);
        }

        [Fact]
        public void Providing_inputs_calls_input_methods_with_matching_outputs()
        {
            var sut = new OperationContext();
            var outputOperation = new OutputOperation<object>() { OutputValue = new object() };
            var inputOperation = new InputOperation<object>();
            sut.RegisterOutputHandlers(outputOperation);
            outputOperation.Execute();

            sut.ProvideInputs(inputOperation);

            Assert.Equal(outputOperation.OutputValue, inputOperation.ProvidedInput);
        }

        private class OutputOperation<TOutput> : Operation, IOutputOperation<TOutput>
        {
            public Action<TOutput> OnReceiveOutput { get; private set; }
            public TOutput OutputValue { get; set; }

            protected override void OnExecute()
            {
                OnReceiveOutput(OutputValue);
            }

            public void Output(Action<TOutput> onReceiveOutput)
            {
                OnReceiveOutput = onReceiveOutput;
            }
        }

        private class InputOperation<TInput> : Operation, IInputOperation<TInput>
        {
            public TInput ProvidedInput { get; private set; }

            protected override void OnExecute() { }

            public void Input(TInput input)
            {
                ProvidedInput = input;
            }
        }
    }
}
