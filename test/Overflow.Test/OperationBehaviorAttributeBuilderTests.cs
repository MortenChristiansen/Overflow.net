using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationBehaviorAttributeBuilderTests
    {
        [Fact]
        public void Behaviors_are_applied_to_operations_with_the_operation_behavior_attribute()
        {
            var sut = new OperationBehaviorAttributeBuilder();
            var operation = new BehaviorOperation();

            var result = sut.ApplyBehavior(operation, new WorkflowConfiguration());

            Assert.IsType<FakeOperationBehavior>(result);
            Assert.IsType<BehaviorOperation>((result as OperationBehavior).InnerOperation);
        }

        [Fact]
        public void Operations_with_no_behavior_attributes_are_not_decorated()
        {
            var sut = new OperationBehaviorAttributeBuilder();
            var operation = new FakeOperation();

            var result = sut.ApplyBehavior(operation, new WorkflowConfiguration());

            Assert.Equal(operation, result);
        }

        [Fact]
        public void You_cannot_apply_behavior_without_an_operation()
        {
            var sut = new OperationBehaviorAttributeBuilder();

            Assert.Throws<ArgumentNullException>(() => sut.ApplyBehavior(null, new WorkflowConfiguration()));
        }

        [FakeOperationBehavior]
        private class BehaviorOperation : Operation
        {
            protected override void OnExecute() { }
        }
    }
}
