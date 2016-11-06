using System;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;

namespace Overflow.Test
{
    public class IOerationTests
    {
        [Theory, AutoMoqData]
        public void Gettting_executed_operations_for_hierarchy_returns_all_executed_tasks_in_the_order_executed(IOperation grandChildOperation, IOperation childOperation2)
        {
            var childOperation1 = new FakeOperation(grandChildOperation);
            var sut = new FakeOperation(childOperation1, childOperation2);
            sut.Execute();

            var executions = sut.GetExecutedChildOperationsForOperationHierarchy();

            Assert.Equal(3, executions.Count);
            Assert.Equal(childOperation1, executions[0].Operation);
            Assert.Equal(grandChildOperation, executions[1].Operation);
            Assert.Equal(childOperation2, executions[2].Operation);
        }

        [Fact]
        public void A_behaviorless_operation_is_its_own_innermost_operation()
        {
            var sut = new FakeOperation();

            var result = sut.GetInnermostOperation();

            Assert.Equal(sut, result);
        }

        [Theory, AutoMoqData]
        public void The_inner_operation_of_a_behavior_is_identified_as_the_innermost_operation(IOperation operation)
        {
            var sut = new FakeOperationBehavior().AttachTo(operation);

            var result = sut.GetInnermostOperation();

            Assert.Equal(operation, result);
        }

        [Theory, AutoMoqData]
        public void The_inner_operation_of_nested_behaviors_is_identified_as_the_innermost_operation(IOperation operation)
        {
            var behavior = new FakeOperationBehavior().AttachTo(operation);
            var sut = new FakeOperationBehavior().AttachTo(behavior);

            var result = sut.GetInnermostOperation();

            Assert.Equal(operation, result);
        }

        [Theory, AutoMoqData]
        public void You_can_provide_input_to_an_operation(FakeInputOperation<object> operation, object input)
        {
            operation.ProvideInput(input);

            Assert.Same(input, operation.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void Inputs_Are_provided_to_the_innermost_operation(FakeInputOperation<object> operation, object input)
        {
            var outerOperation = new FakeOperationBehavior().AttachTo(operation);

            outerOperation.ProvideInput(input);

            Assert.Same(input, operation.ProvidedInput);
        }

        [Theory, AutoMoqData]
        public void The_outer_operation_is_returned_when_providing_input(FakeInputOperation<object> operation, object input)
        {
            var outerOperation = new FakeOperationBehavior().AttachTo(operation);

            var result = outerOperation.ProvideInput(input);

            Assert.Same(outerOperation, result);
        }

        [Theory, AutoMoqData]
        public void You_cannot_provide_input_for_which_there_is_no_input_attribute(FakeInputOperation<IOperation> operation, object input)
        {
            Assert.Throws<ArgumentException>(() => operation.ProvideInput(input));
        }
    }
}
