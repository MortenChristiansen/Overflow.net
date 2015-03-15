using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class IOerationTests
    {
        [Fact]
        public void Gettting_executed_operations_for_hierarchy_returns_all_executed_tasks_in_the_order_executed()
        {
            var grandChildOperation = new FakeOperation();
            var childOperation1 = new FakeOperation(grandChildOperation);
            var childOperation2 = new FakeOperation();
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

        [Fact]
        public void The_inner_operation_of_a_behavior_is_identified_as_the_innermost_operation()
        {
            var operation = new FakeOperation();
            var sut = new FakeOperationBehavior().Attach(operation);

            var result = sut.GetInnermostOperation();

            Assert.Equal(operation, result);
        }

        [Fact]
        public void The_inner_operation_of_nested_behaviors_is_identified_as_the_innermost_operation()
        {
            var operation = new FakeOperation();
            var behavior = new FakeOperationBehavior().Attach(operation);
            var sut = new FakeOperationBehavior().Attach(behavior);

            var result = sut.GetInnermostOperation();

            Assert.Equal(operation, result);
        }
    }
}
