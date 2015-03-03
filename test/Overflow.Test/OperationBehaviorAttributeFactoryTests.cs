using System;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationBehaviorAttributeFactoryTests
    {
        [Fact]
        public void Behaviors_are_created_for_the_available_behavior_attributes()
        {
            var sut = new OperationBehaviorAttributeFactory();
            var operation = new BehaviorOperation();

            var result = sut.CreateBehaviors(operation, new FakeWorkflowConfiguration());

            Assert.Equal(1, result.Count);
            Assert.IsType<FakeOperationBehavior>(result[0]);
        }

        [Fact]
        public void No_behaviors_are_created_when_there_are_no_behavior_attributes()
        {
            var sut = new OperationBehaviorAttributeFactory();
            var operation = new FakeOperation();

            var result = sut.CreateBehaviors(operation, new FakeWorkflowConfiguration());

            Assert.Equal(0, result.Count);
        }

        [Fact]
        public void You_cannot_create_behaviors_without_an_operation()
        {
            var sut = new OperationBehaviorAttributeFactory();

            Assert.Throws<ArgumentNullException>(() => sut.CreateBehaviors(null, new FakeWorkflowConfiguration()));
        }

        [FakeOperationBehavior]
        private class BehaviorOperation : Operation
        {
            protected override void OnExecute() { }
        }
    }
}
