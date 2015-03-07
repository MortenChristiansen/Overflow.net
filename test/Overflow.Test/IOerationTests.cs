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
    }
}
