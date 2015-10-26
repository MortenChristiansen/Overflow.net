using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;

namespace Overflow.Test
{
    public class OperationContextTests : TestBase
    {
        [Fact]
        public void Guards_are_verified()
        {
            VerifyGuards<OperationContext>();
        }

        [Theory, AutoMoqData]
        public void Registering_output_handlers_calls_registration_methods_on_output_operation(IOperation op)
        {
            var sut = OperationContext.Create(op);
            var operation = new FakeOutputOperation<object>();

            sut.RegisterOutputHandlers(operation);

            Assert.NotNull(operation.OnReceiveOutput);
        }

        [Theory, AutoMoqData]
        public void Data_flows_from_output_to_input_operations(IOperation op, object output)
        {
            var sut = OperationContext.Create(op);
            var outputOperation = new FakeOutputOperation<object> { OutputValue = output };
            var inputOperation = new FakeInputOperation<object>();
            sut.RegisterOutputHandlers(outputOperation);

            outputOperation.Execute();
            sut.ProvideInputs(inputOperation);

            Assert.Equal(outputOperation.OutputValue, inputOperation.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void The_most_recently_outputted_instance_of_a_data_type_is_available_as_input(IOperation op, object output)
        {
            var sut = OperationContext.Create(op);
            var outputOperation1 = new FakeOutputOperation<object> { OutputValue = output };
            var outputOperation2 = new FakeOutputOperation<object> { OutputValue = output };
            var inputOperation = new FakeInputOperation<object>();
            sut.RegisterOutputHandlers(outputOperation1);
            sut.RegisterOutputHandlers(outputOperation2);

            outputOperation1.Execute();
            outputOperation2.Execute();
            sut.ProvideInputs(inputOperation);

            Assert.Equal(outputOperation2.OutputValue, inputOperation.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void Input_data_is_not_provided_when_no_output_is_available(IOperation op, FakeInputOperation<object> inputOperation)
        {
            var sut = OperationContext.Create(op);

            sut.ProvideInputs(inputOperation);

            Assert.False(inputOperation.InputWasProvided);
        }

        [Theory, AutoMoqData]
        public void Data_flows_from_output_to_multiple_input_operations(IOperation op, object output)
        {
            var sut = OperationContext.Create(op);
            var outputOperation = new FakeOutputOperation<object> { OutputValue = output };
            var inputOperation1 = new FakeInputOperation<object>();
            var inputOperation2 = new FakeInputOperation<object>();
            sut.RegisterOutputHandlers(outputOperation);

            outputOperation.Execute();
            sut.ProvideInputs(inputOperation1);
            sut.ProvideInputs(inputOperation2);

            Assert.Equal(outputOperation.OutputValue, inputOperation1.ProvidedInput);
            Assert.Equal(outputOperation.OutputValue, inputOperation2.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void Data_flows_from_the_most_recent_output_to_the_following_input_operation(IOperation op, object output)
        {
            var sut = OperationContext.Create(op);
            var outputOperation1 = new FakeOutputOperation<object> { OutputValue = output };
            var outputOperation2 = new FakeOutputOperation<object> { OutputValue = output };
            var inputOperation1 = new FakeInputOperation<object>();
            var inputOperation2 = new FakeInputOperation<object>();
            sut.RegisterOutputHandlers(outputOperation1);
            sut.RegisterOutputHandlers(outputOperation2);

            outputOperation1.Execute();
            sut.ProvideInputs(inputOperation1);
            outputOperation2.Execute();
            sut.ProvideInputs(inputOperation2);

            Assert.Equal(outputOperation1.OutputValue, inputOperation1.ProvidedInput);
            Assert.Equal(outputOperation2.OutputValue, inputOperation2.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void You_can_get_output_directly_from_the_context(IOperation op, object output)
        {
            var sut = OperationContext.Create(op);
            var outputOperation = new FakeOutputOperation<object> { OutputValue = output };
            sut.RegisterOutputHandlers(outputOperation);
            outputOperation.Execute();

            var result = sut.GetOutput<object>();

            Assert.Equal(outputOperation.OutputValue, result);
        }

        [Theory, AutoMoqData]
        public void You_cannot_get_output_of_a_specialized_type_directly_from_the_context(IOperation op, string output)
        {
            var sut = OperationContext.Create(op);
            var outputOperation = new FakeOutputOperation<string> { OutputValue = output };
            sut.RegisterOutputHandlers(outputOperation);
            outputOperation.Execute();

            var result = sut.GetOutput<object>(allowSpecializedClasses: false);

            Assert.Null(result);
        }

        [Theory, AutoMoqData]
        public void You_can_get_output_of_a_specialized_type_directly_from_the_context_if_you_ask_for_it_explicitly(IOperation op, string output)
        {
            var sut = OperationContext.Create(op);
            var outputOperation = new FakeOutputOperation<string> { OutputValue = output };
            sut.RegisterOutputHandlers(outputOperation);
            outputOperation.Execute();

            var result = sut.GetOutput<object>(allowSpecializedClasses: true);

            Assert.Equal(outputOperation.OutputValue, result);
        }

        [Theory, AutoMoqData]
        public void Getting_an_unavailable_output_type_from_the_context_returns_null(IOperation op)
        {
            var sut = OperationContext.Create(op);

            var result = sut.GetOutput<object>();

            Assert.Null(result);
        }

        [Theory, AutoMoqData]
        public void Creating_an_operation_context_returns_new_instance(IOperation op)
        {
            var result = OperationContext.Create(op);

            Assert.NotNull(result);
        }

        [Theory, AutoMoqData]
        public void Registering_output_handlers_calls_registration_methods_on_decorated_output_operation(IOperation op, FakeOutputOperation<object> decoratedOperation)
        {
            var sut = OperationContext.Create(op);
            var operation = new FakeOperationBehavior().AttachTo(decoratedOperation);

            sut.RegisterOutputHandlers(operation);

            Assert.NotNull(decoratedOperation.OnReceiveOutput);
        }

        [Theory, AutoMoqData]
        public void Data_flows_from_output_to_decorated_input_operations(IOperation op, FakeInputOperation<object> inputOperation, object output)
        {
            var sut = OperationContext.Create(op);
            var outputOperation = new FakeOutputOperation<object> { OutputValue = output };
            var decoratedInputOperation = new FakeOperationBehavior().AttachTo(inputOperation);
            sut.RegisterOutputHandlers(outputOperation);

            outputOperation.Execute();
            sut.ProvideInputs(decoratedInputOperation);

            Assert.Equal(outputOperation.OutputValue, inputOperation.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void Registering_output_handlers_calls_registration_methods_on_nested_decorated_output_operation(FakeOutputOperation<object> decoratedOperation)
        {
            var sut = OperationContext.Create(new FakeOperation());
            var operation = new FakeOperationBehavior().AttachTo(new FakeOperationBehavior().AttachTo(decoratedOperation));

            sut.RegisterOutputHandlers(operation);

            Assert.NotNull(decoratedOperation.OnReceiveOutput);
        }

        [Theory, AutoMoqData]
        public void Data_flows_from_output_to_nested_decorated_input_operations(IOperation op, object output, FakeInputOperation<object> inputOperation)
        {
            var sut = OperationContext.Create(op);
            var outputOperation = new FakeOutputOperation<object> { OutputValue = output };
            var decoratedInputOperation = new FakeOperationBehavior().AttachTo(new FakeOperationBehavior().AttachTo(inputOperation));
            sut.RegisterOutputHandlers(outputOperation);

            outputOperation.Execute();
            sut.ProvideInputs(decoratedInputOperation);

            Assert.Equal(outputOperation.OutputValue, inputOperation.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void Providing_inputs_sets_input_properties(IOperation op, object input)
        {
            var sut = OperationContext.Create(op);
            sut.AddData(input);
            var inputOperation = new TestInputOperation();

            sut.ProvideInputs(inputOperation);

            Assert.Equal(input, inputOperation.Input);
        }

        [Theory, AutoMoqData]
        public void Providing_inputs_to_a_decorated_operation_sets_input_properties(IOperation op, object input)
        {
            var sut = OperationContext.Create(op);
            sut.AddData(input);
            var innerOperation = new TestInputOperation();
            var inputOperation = new FakeOperationBehavior().AttachTo(innerOperation);

            sut.ProvideInputs(inputOperation);

            Assert.Equal(input, innerOperation.Input);
        }

        [Theory, AutoMoqData]
        public void Providing_inputs_does_not_set_input_when_input_is_missing(IOperation op)
        {
            var sut = OperationContext.Create(op);
            var inputOperation = new TestInputOperation();

            sut.ProvideInputs(inputOperation);

            Assert.Null(inputOperation.Input);
        }

        [Theory, AutoMoqData]
        public void Adding_output_values_updates_the_context_data(IOperation op, object output)
        {
            var sut = OperationContext.Create(op);
            var outputOperation = new TestOutputOperation { Output = output };

            sut.AddOutput(outputOperation);

            Assert.Equal(output, sut.GetOutput<object>());
        }

        [Theory, AutoMoqData]
        public void Adding_output_values_to_a_decorated_operation_updates_the_context_data(IOperation op, object output)
        {
            var sut = OperationContext.Create(op);
            var outputOperation = new TestOutputOperation { Output = output };

            sut.AddOutput(outputOperation);

            Assert.Equal(output, sut.GetOutput<object>());
        }

        [Theory, AutoMoqData]
        public void Adding_output_values_does_not_update_the_context_data_when_not_supplied(IOperation op)
        {
            var sut = OperationContext.Create(op);
            var outputOperation = new FakeOperationBehavior().AttachTo(new TestOutputOperation { Output = null });

            sut.AddOutput(outputOperation);

            Assert.Null(sut.GetOutput<object>());
        }

        [Theory, AutoMoqData]
        public void Inputs_can_be_piped_to_child_operations(object input)
        {
            var innerInputOperation = new TestInputOperation();
            var inputOperation = new TestPipedInputOperation(innerInputOperation) { Input = input };
            var op = new FakeOperation(inputOperation);
            var sut = OperationContext.Create(op);
            sut.AddData(input);
            
            op.Execute();

            Assert.Same(input, innerInputOperation.Input);
        }

        private class TestInputOperation : Operation
        {
            [Input]
            public object Input { get; set; }
        }

        private class TestOutputOperation : Operation
        {
            [Output]
            public object Output { get; set; }
        }

        private class TestPipedInputOperation : FakeOperation
        {
            [Input, Pipe]
            public object Input { get; set; }

            public TestPipedInputOperation(params IOperation[] childOperations)
                : base(childOperations)
            { }
        }

        private class TestPipedOutputOperation : FakeOperation
        {
            [Output, Pipe]
            public object Output { get; set; }

            public TestPipedOutputOperation(params IOperation[] childOperations)
                : base(childOperations)
            { }
        }
    }
}
