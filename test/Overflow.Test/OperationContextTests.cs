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
        public void Data_flows_from_output_to_input_operations()
        {
            var sut = new OperationContext();
            var outputOperation = new OutputOperation<object> { OutputValue = new object() };
            var inputOperation = new InputOperation<object>();
            sut.RegisterOutputHandlers(outputOperation);

            outputOperation.Execute();
            sut.ProvideInputs(inputOperation);

            Assert.Equal(outputOperation.OutputValue, inputOperation.ProvidedInput);
        }

        [Fact]
        public void The_most_recently_outputted_instance_of_a_data_type_is_available_as_input()
        {
            var sut = new OperationContext();
            var outputOperation1 = new OutputOperation<object> { OutputValue = new object() };
            var outputOperation2 = new OutputOperation<object> { OutputValue = new object() };
            var inputOperation = new InputOperation<object>();
            sut.RegisterOutputHandlers(outputOperation1);
            sut.RegisterOutputHandlers(outputOperation2);

            outputOperation1.Execute();
            outputOperation2.Execute();
            sut.ProvideInputs(inputOperation);

            Assert.Equal(outputOperation2.OutputValue, inputOperation.ProvidedInput);
        }

        [Fact]
        public void Input_data_is_null_when_no_output_is_available()
        {
            var sut = new OperationContext();
            var inputOperation = new InputOperation<object>();

            sut.ProvideInputs(inputOperation);

            Assert.Null(inputOperation.ProvidedInput);
        }

        [Fact]
        public void Data_flows_from_output_to_multiple_input_operations()
        {
            var sut = new OperationContext();
            var outputOperation = new OutputOperation<object> { OutputValue = new object() };
            var inputOperation1 = new InputOperation<object>();
            var inputOperation2 = new InputOperation<object>();
            sut.RegisterOutputHandlers(outputOperation);

            outputOperation.Execute();
            sut.ProvideInputs(inputOperation1);
            sut.ProvideInputs(inputOperation2);

            Assert.Equal(outputOperation.OutputValue, inputOperation1.ProvidedInput);
            Assert.Equal(outputOperation.OutputValue, inputOperation2.ProvidedInput);
        }

        [Fact]
        public void Data_flows_from_the_most_recent_output_to_the_following_input_operation()
        {
            var sut = new OperationContext();
            var outputOperation1 = new OutputOperation<object> { OutputValue = new object() };
            var outputOperation2 = new OutputOperation<object> { OutputValue = new object() };
            var inputOperation1 = new InputOperation<object>();
            var inputOperation2 = new InputOperation<object>();
            sut.RegisterOutputHandlers(outputOperation1);
            sut.RegisterOutputHandlers(outputOperation2);

            outputOperation1.Execute();
            sut.ProvideInputs(inputOperation1);
            outputOperation2.Execute();
            sut.ProvideInputs(inputOperation2);

            Assert.Equal(outputOperation1.OutputValue, inputOperation1.ProvidedInput);
            Assert.Equal(outputOperation2.OutputValue, inputOperation2.ProvidedInput);
        }

        private class OutputOperation<TOutput> : Operation, IOutputOperation<TOutput> where TOutput : class
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

        private class InputOperation<TInput> : Operation, IInputOperation<TInput> where TInput : class
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
