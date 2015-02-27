using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationContextTests
    {
        [Fact]
        public void Registering_output_handlers_calls_registration_methods_on_output_operation()
        {
            var sut = OperationContext.Create(new FakeOperation());
            var operation = new FakeOutputOperation<object>();

            sut.RegisterOutputHandlers(operation);

            Assert.NotNull(operation.OnReceiveOutput);
        }

        [Fact]
        public void Data_flows_from_output_to_input_operations()
        {
            var sut = OperationContext.Create(new FakeOperation());
            var outputOperation = new FakeOutputOperation<object> { OutputValue = new object() };
            var inputOperation = new FakeInputOperation<object>();
            sut.RegisterOutputHandlers(outputOperation);

            outputOperation.Execute();
            sut.ProvideInputs(inputOperation);

            Assert.Equal(outputOperation.OutputValue, inputOperation.ProvidedInput);
        }

        [Fact]
        public void The_most_recently_outputted_instance_of_a_data_type_is_available_as_input()
        {
            var sut = OperationContext.Create(new FakeOperation());
            var outputOperation1 = new FakeOutputOperation<object> { OutputValue = new object() };
            var outputOperation2 = new FakeOutputOperation<object> { OutputValue = new object() };
            var inputOperation = new FakeInputOperation<object>();
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
            var sut = OperationContext.Create(new FakeOperation());
            var inputOperation = new FakeInputOperation<object>();

            sut.ProvideInputs(inputOperation);

            Assert.Null(inputOperation.ProvidedInput);
        }

        [Fact]
        public void Data_flows_from_output_to_multiple_input_operations()
        {
            var sut = OperationContext.Create(new FakeOperation());
            var outputOperation = new FakeOutputOperation<object> { OutputValue = new object() };
            var inputOperation1 = new FakeInputOperation<object>();
            var inputOperation2 = new FakeInputOperation<object>();
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
            var sut = OperationContext.Create(new FakeOperation());
            var outputOperation1 = new FakeOutputOperation<object> { OutputValue = new object() };
            var outputOperation2 = new FakeOutputOperation<object> { OutputValue = new object() };
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

        [Fact]
        public void You_can_get_output_directly_from_the_context()
        {
            var sut = OperationContext.Create(new FakeOperation());
            var outputOperation = new FakeOutputOperation<object> { OutputValue = new object() };
            sut.RegisterOutputHandlers(outputOperation);
            outputOperation.Execute();

            var result = sut.GetOutput<object>();

            Assert.Equal(outputOperation.OutputValue, result);
        }

        [Fact]
        public void Getting_an_unavailable_output_type_from_the_context_returns_null()
        {
            var sut = OperationContext.Create(new FakeOperation());

            var result = sut.GetOutput<object>();

            Assert.Null(result);
        }

        [Fact]
        public void Creating_an_operation_context_returns_new_instance()
        {
            var result = OperationContext.Create(new FakeOperation());

            Assert.NotNull(result);
        }

        [Fact]
        public void Registering_output_handlers_calls_registration_methods_on_decorated_output_operation()
        {
            var sut = OperationContext.Create(new FakeOperation());
            var decoratedOperation = new FakeOutputOperation<object>();
            var operation = new FakeOperationDecorator(decoratedOperation);

            sut.RegisterOutputHandlers(operation);

            Assert.NotNull(decoratedOperation.OnReceiveOutput);
        }

        [Fact]
        public void Data_flows_from_output_to_decorated_input_operations()
        {
            var sut = OperationContext.Create(new FakeOperation());
            var outputOperation = new FakeOutputOperation<object> { OutputValue = new object() };
            var inputOperation = new FakeInputOperation<object>();
            var decoratedInputOperation = new FakeOperationDecorator(inputOperation);
            sut.RegisterOutputHandlers(outputOperation);

            outputOperation.Execute();
            sut.ProvideInputs(decoratedInputOperation);

            Assert.Equal(outputOperation.OutputValue, inputOperation.ProvidedInput);
        }

        [Fact]
        public void Registering_output_handlers_calls_registration_methods_on_nested_decorated_output_operation()
        {
            var sut = OperationContext.Create(new FakeOperation());
            var decoratedOperation = new FakeOutputOperation<object>();
            var operation = new FakeOperationDecorator(new FakeOperationDecorator(decoratedOperation));

            sut.RegisterOutputHandlers(operation);

            Assert.NotNull(decoratedOperation.OnReceiveOutput);
        }

        [Fact]
        public void Data_flows_from_output_to_nested_decorated_input_operations()
        {
            var sut = OperationContext.Create(new FakeOperation());
            var outputOperation = new FakeOutputOperation<object> { OutputValue = new object() };
            var inputOperation = new FakeInputOperation<object>();
            var decoratedInputOperation = new FakeOperationDecorator(new FakeOperationDecorator(inputOperation));
            sut.RegisterOutputHandlers(outputOperation);

            outputOperation.Execute();
            sut.ProvideInputs(decoratedInputOperation);

            Assert.Equal(outputOperation.OutputValue, inputOperation.ProvidedInput);
        }
    }
}
