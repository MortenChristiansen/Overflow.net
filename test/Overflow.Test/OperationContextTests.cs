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
    }
}
