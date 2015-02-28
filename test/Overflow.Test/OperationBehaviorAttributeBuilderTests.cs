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

            var result = sut.ApplyBehavior(operation);

            Assert.IsType<FakeOperationBehavior>(result);
            Assert.IsType<BehaviorOperation>((result as OperationBehavior).InnerOperation);
        }

        [Fact]
        public void Operations_with_no_behavior_attributes_are_not_decorated()
        {
            var sut = new OperationBehaviorAttributeBuilder();
            var operation = new FakeOperation();

            var result = sut.ApplyBehavior(operation);

            Assert.Equal(operation, result);
        }

        [FakeOperationBehavior]
        private class BehaviorOperation : Operation
        {
            protected override void OnExecute() { }
        }
    }
}
